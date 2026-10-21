using Domain.Entities.Inventory;
using Domain.Events.Inventory;
using Domain.ValueObjects.Inventory;

namespace Domain.Aggregates
{
    public class InventoryAggregate : Aggregate
    {
        public InventoryItem CurrentInventoryItem { get; set; } = new();

        public void AddInventoryItem(int eventVersion, InventoryItemIdentity inventoryItemIdentity, int Quantity)
        {
            if (Quantity < 0)
            {
                throw new InvalidOperationException("Quantity to remove must be a positive number.");
            }

            var inventoryItemAdded = new InventoryAdded
            {
                Version = eventVersion,
                InventoryEvent = new InventoryEvent(eventVersion, inventoryItemIdentity),
                Quantity = Quantity
            };

            AddEvent(inventoryItemAdded.InventoryEvent);

            Apply(inventoryItemAdded);

            commit();
        }

        public void RemoveInventoryItem(int eventVersion, InventoryItemIdentity inventoryItemIdentity, int Quantity)
        {
            if(CurrentInventoryItem.Quantity < Quantity)
            {
                throw new InvalidOperationException("Insufficient inventory to remove the specified quantity.");
            }

            if(Quantity < 0)
            {
                throw new InvalidOperationException("Quantity to remove must be a positive number.");
            }

            var inventoryItemRemoved = new InventoryRemoved()
            {
                Version = eventVersion,
                InventoryEvent = new InventoryEvent(eventVersion, inventoryItemIdentity)
            };
            //(eventVersion, inventoryItemIdentity, Quantity);

            AddEvent(inventoryItemRemoved.InventoryEvent);

            Apply(inventoryItemRemoved);

            commit();
        }


        public void Apply(InventoryAdded inventoryAdded)
        {
            CurrentInventoryItem.Quantity += inventoryAdded.Quantity;
        }

        public void Apply(InventoryRemoved inventoryRemoved)
        {
            CurrentInventoryItem.Quantity -= inventoryRemoved.Quantity;
        }
    }
}
