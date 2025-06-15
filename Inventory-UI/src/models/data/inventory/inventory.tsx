export interface InventoryItemQuantity {
    ProductId: ProductId;
    Quantity: number;
}

export type ProductId = string;

export interface InventoryItem {
    productId?: string; // Specific or Generated Product ID
    name: string;
    description?: string;
    initialQuantity: number;
    price: number;
}

export const InventoryItemKeys: string[] = ["name", "description", "quantity", "price"];

export function isInventoryItem(item: unknown): item is InventoryItem {
    if (typeof item !== 'object' || item === null) return false;

    const inventoryItem = item as InventoryItem;
    return (
        typeof inventoryItem.productId === 'string' &&
        typeof inventoryItem.name === 'string' &&
        (typeof inventoryItem.description === 'undefined' || typeof inventoryItem.description === 'string') &&
        typeof inventoryItem.initialQuantity === 'number' &&
        typeof inventoryItem.price === 'number'
    );
}