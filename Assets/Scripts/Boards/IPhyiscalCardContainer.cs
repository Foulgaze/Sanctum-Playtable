using System.Collections.Generic;
using System.Numerics;
using Sanctum_Core;

public interface IPhysicalCardContainer
{
    public CardZone GetZone();
    public void Setup(CardZone zone, bool isOpponent);
    public bool IsOpponent();
    public void OnCardAdded(NetworkAttribute attribute);
    public void UpdateHolder(List<List<int>> boardDescription);
    public void AddCard(int cardId);
    public void RerenderContainer();
    public void FlipTopCard(NetworkAttribute value);
    public bool RevealTopCard();


}