using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MapConfig", order = 1)]
public class MapConfig : ScriptableObject
{
    public GameObject levelPrefab;
    public Texture2D texture;

    private Sprite _sprite;

    public Sprite GetSprite()
    {
        if(_sprite == null)
        {
            _sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        }
        return _sprite;
    }
}