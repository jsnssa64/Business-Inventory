export interface Product {
    Name: string;
    Description: string;
    Price?: number;
    Currency?: string;
    ItemQuantity: number;
    InventoryQuantity: number;
    EnabledPrice: boolean;
}

export interface UpdateProductDetails 
{
    ProductId: string;
    ProductName?: string | null;
    Description?: string | null;
    Quantity: number;
    Price?: number | null;
    CurrencyCode?: string | null;
}

export type ProductId = string;