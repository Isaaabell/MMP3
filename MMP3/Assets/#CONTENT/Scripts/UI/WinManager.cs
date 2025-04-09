using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _yourScoreCanvasGroup;
    [SerializeField] private CanvasGroup _highScoreCanvasGroup;
    [SerializeField] private CanvasGroup _lockScreenCanvasGroup;
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private TMP_Text _yourScoreText;
    [SerializeField] private HighScoreHandler _highScoreHandler;

    [SerializeField] private GameObject _highScoreElementPrefab;
    [SerializeField] private Transform _highScoreParent;
    List<GameObject> _uiElements = new List<GameObject>();
    public bool inputEnabled = true;
    public Animator laptopAnimator;

    private void OnEnable()
    {
        HighScoreHandler.onHighScoreListChanged += UpdateUI;
    }

    private void OnDisable()
    {
        HighScoreHandler.onHighScoreListChanged -= UpdateUI;
    }

    void Awake()
    {
        DisableInput();
        ToggleCanvasGroup(_yourScoreCanvasGroup, false);
        ToggleCanvasGroup(_highScoreCanvasGroup, false);
        laptopAnimator.SetBool("isOpen", true);

    }

    void Update()
    {
        if (!inputEnabled) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleCanvasGroup(_lockScreenCanvasGroup, false);
            ToggleCanvasGroup(_yourScoreCanvasGroup, true);
        }
    }

    public void EnableInput()
    {
        inputEnabled = true;
    }

    public void DisableInput()
    {
        inputEnabled = false;
    }

    public void DoneBtn()
    {
        ToggleCanvasGroup(_yourScoreCanvasGroup, false);
        ToggleCanvasGroup(_highScoreCanvasGroup, true);

        int score;
        int.TryParse(_yourScoreText.text, out score);
        _highScoreHandler.AddHighScoreIfPossible(new HighScoreElement(2, _nameInputField.text, score));
    }

    public void UpdateUI(List<HighScoreElement> highScoreList)
    {
        for (int i = 0; i < highScoreList.Count; i++)
        {
            HighScoreElement el = highScoreList[i];
            if (el.score > 0)
            {
                if (i >= _uiElements.Count)
                {
                    var inst = Instantiate(_highScoreElementPrefab, Vector3.zero, Quaternion.identity);
                    inst.transform.SetParent(_highScoreParent, false);

                    _uiElements.Add(inst);
                }

                var texts = _uiElements[i].GetComponentsInChildren<TMP_Text>();
                texts[0].text = (i + 1).ToString(); // Update place to be 1-based index
                texts[1].text = el.name;
                texts[2].text = el.score.ToString();
            }
        }
    }
    private void ToggleCanvasGroup(CanvasGroup canvasGroup, bool isActive)
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = isActive ? 1 : 0;
        canvasGroup.interactable = isActive;
        canvasGroup.blocksRaycasts = isActive;
    }
}
