﻿using System.Collections.Generic;
using System.ComponentModel;


public enum CardZone { Library, Graveyard, Exile, CommandZone, MainField, LeftField, RightField, Hand }
public class Player
{
    public string Name { get; }
    public string Uuid { get; }
    public NetworkAttribute<string> DeckListRaw { get; }
    public NetworkAttribute<int> Health { get; }
    public NetworkAttribute<(List<string>, int?, CardZone zone)> revealCardZone { get; set; }
    public NetworkAttribute<bool> ReadiedUp { get; set; }
    private readonly Dictionary<CardZone, CardContainerCollection> zoneToContainer = new();
    private readonly NetworkAttributeFactory networkAttributeFactory;
    private readonly CardFactory cardFactory;
    public event PropertyChangedEventHandler boardChanged = delegate { };

    /// <summary>
    /// Initializes a new instance of the <see cref="Player"/> class.
    /// </summary>
    /// <param name="uuid">The unique identifier for the player.</param>
    /// <param name="name">The name of the player.</param>
    /// <param name="startingHealth">The starting health of the player.</param>
    /// <param name="networkAttributeFactory">The factory responsible for creating network attributes.</param>
    /// <param name="cardFactory">The factory responsible for creating cards.</param>
    public Player(string uuid, string name, int startingHealth, NetworkAttributeFactory networkAttributeFactory, CardFactory cardFactory)
    {
        this.Uuid = uuid;
        this.Name = name;
        this.networkAttributeFactory = networkAttributeFactory;
        this.cardFactory = cardFactory;
        this.Health = this.networkAttributeFactory.AddNetworkAttribute<int>($"{this.Uuid}-health", startingHealth);
        this.DeckListRaw = this.networkAttributeFactory.AddNetworkAttribute<string>($"{this.Uuid}-decklist", "");
        this.ReadiedUp = this.networkAttributeFactory.AddNetworkAttribute($"{this.Uuid}-ready", false);
        this.revealCardZone = this.networkAttributeFactory.AddNetworkAttribute<(List<string>, int ?, CardZone zone)>($"{this.Uuid}-reveal", (new(), null, CardZone.Library));
        this.InitializeBoards();
    }

    private void InitializeBoards()
    {
        this.zoneToContainer[CardZone.Library] = new CardContainerCollection(CardZone.Library, this.Uuid, false,this.networkAttributeFactory, this.cardFactory);
        this.zoneToContainer[CardZone.Graveyard] = new CardContainerCollection(CardZone.Graveyard, this.Uuid,true, this.networkAttributeFactory, this.cardFactory);
        this.zoneToContainer[CardZone.Exile] = new CardContainerCollection(CardZone.Exile, this.Uuid, true,this.networkAttributeFactory, this.cardFactory);
        this.zoneToContainer[CardZone.CommandZone] = new CardContainerCollection(CardZone.CommandZone, this.Uuid, true, this.networkAttributeFactory, this.cardFactory);
        this.zoneToContainer[CardZone.Hand] = new CardContainerCollection(CardZone.Hand, this.Uuid,true, this.networkAttributeFactory, this.cardFactory);
        this.zoneToContainer[CardZone.MainField] = new CardContainerCollection(CardZone.MainField, this.Uuid, true,this.networkAttributeFactory, this.cardFactory);
        this.zoneToContainer[CardZone.LeftField] = new CardContainerCollection(CardZone.LeftField, this.Uuid,  true,this.networkAttributeFactory, this.cardFactory);
        this.zoneToContainer[CardZone.RightField] = new CardContainerCollection(CardZone.RightField, this.Uuid,true, this.networkAttributeFactory, this.cardFactory);
        foreach(CardContainerCollection collection in this.zoneToContainer.Values)
        {
            collection.boardChanged += this.BoardChanged;
        }
    }

    private void BoardChanged(object? o, PropertyChangedEventArgs args)
    {
        boardChanged(o, args);
    }

    /// <summary>
    /// Gets cardzone from player
    /// </summary>
    /// <param name="zone">Zone to get</param>
    /// <returns>Requested zone</returns>
    public CardContainerCollection GetCardContainer(CardZone zone)
    {
        return this.zoneToContainer[zone];
    }
}
