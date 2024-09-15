using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Web;
using Sanctum_Core;

public class TextureController : MonoBehaviour
{
    [SerializeField] Sprite cardBack;
    public static TextureController Instance { get; private set; }
    private float timer = 0;
    private float cooldownPeriod = 0.1f; // 10 miliseconds
    private Dictionary<string, Sprite> uuidToSprite = new();
    private Dictionary<string, CardTexture> uuidToTextureable = new();
    private Queue<string> textureQueue = new();

    private void Awake() 
    {         
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    public bool TextureBackOfCard(ITextureable toBeTextured)
    {
        toBeTextured.TextureSelf(toBeTextured.GetCard().CurrentInfo,cardBack);
        return true;
    }
    public bool TextureImage(ITextureable toBeTextured)
    {
        Card card = toBeTextured.GetCard();
        CardInfo info = card.CurrentInfo;
        if(uuidToSprite.ContainsKey(info.uuid))
        {
            toBeTextured.TextureSelf(info, uuidToSprite[info.uuid]);
            return true;
        }

        string filepath = $"Assets/Resources/Textures/{info.uuid}.jpg";
        if (File.Exists(filepath))
        {
            uuidToSprite[info.uuid] = LoadSpriteFromFile(filepath);
            toBeTextured.TextureSelf(info, uuidToSprite[info.uuid]);
            return true;
        }

        if(uuidToTextureable.ContainsKey(info.uuid))
        {
            uuidToTextureable[info.uuid].Enqueue(toBeTextured);
        }
        else
        {
            UnityLogger.Log($"Enqueing {info.name}");

            CardTexture newTexture = new(info, card.isUsingBackSide.Value);
            newTexture.Enqueue(toBeTextured);
            uuidToTextureable[info.uuid] = newTexture;
            textureQueue.Enqueue(info.uuid);
        }
        return false;
    }

    private Sprite LoadSpriteFromFile(string filepath)
    {
        byte[] fileData = File.ReadAllBytes(filepath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        return sprite;
    }
    // TODO make multiple sources that images can be gathered from. 
    public IEnumerator GetSprite(CardInfo info, Queue<ITextureable> toBeTextured, bool usingBackSide)
    {
        UnityLogger.Log("Getting sprite");
        string filepath = $"Assets/Resources/Textures/{info.uuid}.jpg";
        if (File.Exists(filepath))
        {
            uuidToSprite[info.uuid] = LoadSpriteFromFile(filepath);
            TextureCards(info,toBeTextured, uuidToSprite[info.uuid]);
            yield break;
        }

        string face = usingBackSide ? "back" : "front";
        string url = $"https://api.scryfall.com/cards/{info.setCode.ToLower()}/{info.cardNumber}?format=image&version=normal&face={face}";
        string backupUrl = $"https://api.scryfall.com/cards/named?exact={HttpUtility.UrlEncode(name)}&format=image&face={face}";

        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(url);
        yield return textureRequest.SendWebRequest();

        if (textureRequest.result != UnityWebRequest.Result.Success)
        {
            textureRequest = UnityWebRequestTexture.GetTexture(backupUrl);
            yield return textureRequest.SendWebRequest();
            if (textureRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"Both URLS failed to fetch tectures #1={url} #2={backupUrl}"); 
                yield break;
            }
        }
        
        Texture2D myTexture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
        byte[] pngBytes = myTexture.EncodeToPNG();
        File.WriteAllBytes(filepath, pngBytes);

        Sprite sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), Vector2.one * 0.5f);
        uuidToSprite[info.uuid] = sprite;
        TextureCards(info, toBeTextured, sprite);
    }

    void TextureCards(CardInfo info,Queue<ITextureable> cardsToBeTextured, Sprite sprite)
    {
        while(cardsToBeTextured.Count != 0)
        {
            ITextureable textureableObject = cardsToBeTextured.Dequeue();
            textureableObject.TextureSelf(info, sprite);
        }
    }
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > cooldownPeriod)
        {
            timer = 0;
            if(textureQueue.Count != 0)
            {
                string uuid = textureQueue.Dequeue();
                CardTexture cardTexture = uuidToTextureable[uuid];
                uuidToTextureable.Remove(uuid);
                StartCoroutine(GetSprite(cardTexture.info, cardTexture.cardsToBeTextured, cardTexture.usingBackSide));
            }
        }

    }


   
}