using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public abstract class BaseUI : MonoBehaviourPun
{
    private Dictionary<string, RectTransform> _rects = new Dictionary<string, RectTransform>();
    private Dictionary<string, Button> _buttons = new Dictionary<string, Button>();
    private Dictionary<string, TMP_Text> _texts = new Dictionary<string, TMP_Text>();
    private Dictionary<string, TMP_InputField> _inputFields = new Dictionary<string, TMP_InputField>();
    private Dictionary<string, Image> _images = new Dictionary<string, Image>();
    //[SerializeField] protected AudioSource clickAudio;

    [SerializeField] private AudioSource _buttonClickSound;

    private void Awake()
    {
        BindingChildren();
        AddClickAudio();

        AwakeSelf();
    }

    protected virtual void AwakeSelf()
    {

    }

    private void BindingChildren()
    {
        RectTransform[] childrenRect = GetComponentsInChildren<RectTransform>();
        for (int i = 0; i < childrenRect.Length; i++)
        {
            string key = childrenRect[i].name;
            if (key.Contains("!"))
                continue;

            if (!_rects.ContainsKey(key))
            {
                _rects[key] = childrenRect[i];

                Button btn = childrenRect[i].GetComponent<Button>();
                if (btn)
                {
                    if (!_buttons.ContainsKey(key))
                        _buttons[key] = btn;
                }

                TMP_Text txt = childrenRect[i].GetComponent<TMP_Text>();
                if (txt)
                {
                    if (!_texts.ContainsKey(key))
                        _texts[key] = txt;
                }

                TMP_InputField input = childrenRect[i].GetComponent<TMP_InputField>();
                if (input)
                {
                    if (!_texts.ContainsKey(key))
                        _inputFields[key] = input;
                }

                Image img = childrenRect[i].GetComponent<Image>();
                if (img)
                {
                    if (!_images.ContainsKey(key))
                        _images[key] = img;
                }
            }
        }
    }

    private void AddClickAudio()
    {
        try
        {
            _buttonClickSound = GameManager.Resource.Instantiate<AudioSource>("Audio/ButtonUISound");
            _buttonClickSound.transform.SetParent(transform, false);
            foreach (KeyValuePair<string, Button> button in _buttons)
            {
                button.Value.onClick.AddListener(() => { _buttonClickSound.Play(); });
            }
        }
        catch
        {

        }
    }

    public virtual void CloseUI()
    {

    }

    protected bool GetRect(string name, out RectTransform result)
    {
        if (_rects.TryGetValue(name, out result))
            return true;
        return false;
    }

    protected bool GetButton(string name, out Button result)
    {
        if (_buttons.TryGetValue(name, out result))
            return true;
        return false;
    }

    protected bool GetText(string name, out TMP_Text result)
    {
        if (_texts.TryGetValue(name, out result))
            return true;
        return false;
    }

    protected bool GetInputField(string name, out TMP_InputField result)
    {
        if (_inputFields.TryGetValue(name, out result))
            return true;
        return false;
    }

    protected bool GetImage(string name, out Image result)
    {
        if (_images.TryGetValue(name, out result))
            return true;
        return false;
    }
}