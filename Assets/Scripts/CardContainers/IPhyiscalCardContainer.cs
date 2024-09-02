using System.Collections.Generic;
using Sanctum_Core;

public interface IPhysicalCardContainer
{
    public CardZone GetZone();
    public void SetZone(CardZone zone);
    public void OnCardAdded(NetworkAttribute attribute);
    public void UpdateHolder(List<List<int>> boardDescription);
    public void AddCard(int cardId);
    public void RerenderContainer();
    public void RemoveCard(int cardId);
}