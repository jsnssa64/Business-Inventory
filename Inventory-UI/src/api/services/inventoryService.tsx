import formatString from '../../helper/genericHelper';
import { InventoryItemQuantity, ProductId, InventoryItem } from '../../models/data/inventory/inventory';
import axiosInstance from '../axios/axiosInstance';

const urlInventoryPaths = {
    updateInventoryItemQuantity: '/UpdateInventoryItemQuantity',
    updateItemInInventory: '/UpdateItemInInventory',
    getInventory: '/GetInventory',
    getInventoryItemByProductId: '/GetInventoryItemByProductId/{0}',
}

const inventoryService = {
    UpdateInventoryItemQuantity: async (inventoryItemQuantity: InventoryItemQuantity) => {
        try {
            const response = await axiosInstance.post(urlInventoryPaths.updateInventoryItemQuantity, {
                ProductId: inventoryItemQuantity.ProductId,
                Quantity: inventoryItemQuantity.Quantity
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching inventory items:', error);
            throw error;
        }
    },    
    updateItemInInventory: async (updateInventoryItem: InventoryItem) => {
        try {
            const response = await axiosInstance.post(urlInventoryPaths.updateItemInInventory, {
                productId: updateInventoryItem.productId,
                name: updateInventoryItem.name,
                description: updateInventoryItem.description,
                initialQuantity: updateInventoryItem.initialQuantity,
                price: updateInventoryItem.price
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching inventory items:', error);
            throw error;
        }
    },
    getInventory: async () => {
        try {
            const response = await axiosInstance.get(urlInventoryPaths.getInventory);
            return response.data;
        } catch (error) {
            console.error('Error fetching inventory items:', error);
            throw error;
        }
    },
    getInventoryItemByProductId: async (productId: ProductId) => {
        try {
            const response = await axiosInstance.get(formatString(urlInventoryPaths.getInventoryItemByProductId, productId));
            return response.data;
        } catch (error) {
            console.error('Error fetching inventory items:', error);
            throw error;
        }
    }
}

export default inventoryService;