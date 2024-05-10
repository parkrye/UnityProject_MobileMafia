using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.UI;

public class NightSession : Session
{
    public UnityEvent<int> EmoticonEvent = new UnityEvent<int>();

    protected override void AwakeSelf()
    {
        base.AwakeSelf();

        if (GetButton("ButtonTemplate", out var buttonTemplate))
        {
            if (GetRect("Content", out var content))
            {
                var atlas = GameManager.Resource.Load<SpriteAtlas>("Materials/Icon");
                for (int i = 1; i <= 21; i++)
                {
                    var index = i;
                    var button = Instantiate(buttonTemplate, content);
                    button.GetComponent<Image>().sprite = atlas.GetSprite($"Icon ({index})");
                    button.onClick.AddListener(() => OnEmoticonClicked(index));
                }
            }

            buttonTemplate.gameObject.SetActive(false);
        }

        if (GetButton("EmotionButton", out var eButton))
        {
            eButton.onClick.AddListener(() =>
            {
                if (GetRect("Scroll View", out var scrollView))
                    scrollView.gameObject.SetActive(true);
            });
        }

        if (GetRect("Scroll View", out var scrollView))
            scrollView.gameObject.SetActive(false);

        Time = 30f;
    }

    public override void StartSession()
    {
        base.StartSession();


    }

    public override void EndSession()
    {
        base.EndSession();


    }

    private void OnEmoticonClicked(int index)
    {
        EmoticonEvent?.Invoke(index);

        if (GetRect("Scroll View", out var scrollView))
            scrollView.gameObject.SetActive(false);
    }
}
