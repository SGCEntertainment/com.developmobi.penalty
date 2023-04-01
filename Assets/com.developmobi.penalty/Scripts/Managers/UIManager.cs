using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance 
    { 
        get => FindObjectOfType<UIManager>(); 
    }

    private GameObject _gameRef;

    [SerializeField] GameObject menu;
    [SerializeField] GameObject game;

    private void Awake()
    {
        Ball.OnTravelled += (IsGoal) =>
        {
            if(IsGoal)
            {
                var goalRef = Instantiate(Resources.Load<GameObject>("goal"), game.transform);
                Destroy(goalRef, 2.6f);
            }
            else
            {
                var goalRef = Instantiate(Resources.Load<GameObject>("miss"), game.transform);
                Destroy(goalRef, 2.6f);
            }
        };
    }


    private void Start()
    {
        OpenMenu();
    }

    public void StartGame()
    {
        var _parent = GameObject.Find("Environment").transform;
        var _prefab = Resources.Load<GameObject>("PenaltyGame");

        _gameRef = Instantiate(_prefab, _parent);

        menu.SetActive(false);
        game.SetActive(true);
    }

    public void OpenMenu()
    {
        if(_gameRef)
        {
            Destroy(_gameRef);
        }

        game.SetActive(false);
        menu.SetActive(true);
    }
}
