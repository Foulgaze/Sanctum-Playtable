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