using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


public class CardContainer
{
    public List<Card> Cards { get; } = new();
    private readonly int? maxCardCount;
    private readonly CardZone parentZone;

    /// <summary>
    /// Occurs when the cards collection changes.
    /// </summary>
    public event PropertyChangedEventHandler cardsChanged = delegate { };

    /// <summary>
    /// Initializes a new instance of the <see cref="CardContainer"/> class with the specified zone and owner.
    /// </summary>
    /// <param name="zone">The zone of the card container.</param>
    /// <param name="owner">The owner of the card container.</param>
    public CardContainer(int? maxCardCount, CardZone parentZone)
    {
        this.maxCardCount = maxCardCount;
        this.parentZone = parentZone;
    }

    /// <summary>
    /// Gets the card IDs
    /// </summary>
    /// <returns>A list of all the card ids</returns>
    public List<int> GetCardIDs()
    {
        return this.Cards.Select(card => card.Id).ToList();
    }


    private bool ContainerIsOnField()
    {
        return this.parentZone is CardZone.MainField or CardZone.RightField or CardZone.LeftField;
    }
}