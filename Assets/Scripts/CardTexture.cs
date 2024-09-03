using System.Collections.Generic;
using Sanctum_Core;

public class CardTexture
{
    public Queue<ITextureable> cardsToBeTextured {get;} = new Queue<ITextureable>();
    public CardInfo info {get;}
	public bool usingBackSide;

    public CardTexture(CardInfo info, bool usingBackSide)
    {
        this.info = info;
		this.usingBackSide = usingBackSide;
    }

    public void Enqueue(ITextureable textureable)
    {
        cardsToBeTextured.Enqueue(textureable);
    }

    public ITextureable Dequeue()
    {
        return cardsToBeTextured.Dequeue();
    }
}