using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;



public class InsertCardData
{
    public int? insertPosition;
    public int cardId;
    public int? containerInsertPosition;
    public bool createNewContainer;
    public InsertCardData(int? insertPosition, int cardID, int? containerInsertPosition, bool createNewContainer)
    {
        this.insertPosition = insertPosition;
        this.cardId = cardID;
        this.containerInsertPosition = containerInsertPosition;
        this.createNewContainer = createNewContainer;
    }
}
public class CardContainerCollection
{

    public CardZone Zone { get; set; }
    public string Owner { get; }
    private readonly NetworkAttribute<InsertCardData> insertCardData;
    public NetworkAttribute<bool> revealTopCard;
    public event PropertyChangedEventHandler boardChanged = delegate { };
    private readonly CardFactory CardFactory;
    public List<List<int>> decklist = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CardContainerCollection"/> class with the specified parameters.
    /// </summary>
    /// <param name="zone">The card zone associated with this container collection.</param>
    /// <param name="owner">The owner of the card container collection.</param>
    /// <param name="revealTopCard">Indicates whether the top card of each container should be revealed.</param>
    /// <param name="networkAttributeManager">The factory for managing network attributes.</param>
    /// <param name="cardFactory">The factory for creating cards.</param>
    public CardContainerCollection(CardZone zone, string owner, bool revealTopCard, NetworkAttributeFactory networkAttributeManager, CardFactory cardFactory)
    {
        this.Zone = zone;
        this.Owner = owner;
        this.insertCardData = networkAttributeManager.AddNetworkAttribute($"{owner}-{(int)this.Zone}-insert", new InsertCardData(null, 0, null, false), true, false);
        this.revealTopCard = networkAttributeManager.AddNetworkAttribute($"{owner}-{(int)this.Zone}-reveal", revealTopCard);
        this.CardFactory = cardFactory;
    }

    public void SetCollectionValue(List<List<int>> newDeckList)
    {
        this.decklist = newDeckList;
        boardChanged(this, new PropertyChangedEventArgs("Decklist"));
    }

    /// <summary>
    /// Gets the name of the card zone.
    /// </summary>
    /// <returns>The name of the card zone.</returns>
    public string GetName()
    {
        string? name = Enum.GetName(typeof(CardZone), this.Zone) ?? "Unable To Find";
        return name;
    }
}