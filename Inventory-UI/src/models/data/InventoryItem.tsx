export interface InventoryItem {
    id: string;
    name: string;
    description?: string;
    quantity: number;
    price: number;
}

export const InventoryItemKeys: string[] = ["name", "description", "quantity", "price"];

export function isInventoryItem(item: unknown): item is InventoryItem {
        if (typeof item !== 'object' || item === null) return false;

        const inventoryItem = item as InventoryItem;
        return (
            typeof inventoryItem.id === 'string' &&
            typeof inventoryItem.name === 'string' &&
            (typeof inventoryItem.description === 'undefined' || typeof inventoryItem.description === 'string') &&
            typeof inventoryItem.quantity === 'number' &&
            typeof inventoryItem.price === 'number'
        );
    }