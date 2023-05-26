using UnityEngine;
using UnityEngine.UI;

public class IconScript : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] _sprites;
    public bool IconActive { get; set; }
    void Start()
    {
        _image = GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        SetColor();
    }

    /*
     * Changes the Icon to the defined index.
     * Set Colour switches between white and grey for the UI. Used to show the player which abilities they can use. Grey'd out means the player doesn't have enough Mana.
     */
    public void SpriteChange(int index)
    {
        _image.sprite = _sprites[index];
    }
    private void SetColor()
    {
        _image.color = IconActive ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1);
    }
}
