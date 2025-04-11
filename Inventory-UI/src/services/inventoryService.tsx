import axios from 'axios';
import { InventoryItem } from '../models/data/InventoryItem';

const API_BASE_URL = 'http://localhost:3001/Inventory'; // Replace with your API base URL

const inventoryService = {
    getAllItems: async () => {
        try {
            const response = await axios.get(`${API_BASE_URL}/GetInventoryItems`);
            return response.data;
        } catch (error) {
            console.error('Error fetching inventory items:', error);
            throw error;
        }
    },

    getItemById: async (id: string) => {
        try {
            const response = await axios.get(`${API_BASE_URL}/GetInventoryItemById/${id}`);
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID ${id}:`, error);
            throw error;
        }
    },

    createItem: async (itemData: InventoryItem) => {
        try {
            const response = await axios.post(`${API_BASE_URL}/AddInventoryItem`, itemData);
            return response.data;
        } 
        catch (error) {
            console.error('Error creating inventory item:', error);
            throw error;
        }
    },

    updateItem: async (id: string, itemData: InventoryItem) => {
        try {
            const response = await axios.put(`${API_BASE_URL}/UpdateInventoryItem/${id}`, itemData);
            return response.data;
        } 
        catch (error) {
            console.error(`Error updating item with ID ${id}:`, error);
            throw error;
        }
    },

    deleteItem: async (id: string) => {
        try {
            const response = await axios.delete(`${API_BASE_URL}/RemoveInventoryItem/${id}`);
            return response.data;
        } 
        catch (error) {
            console.error(`Error deleting item with ID ${id}:`, error);
            throw error;
        }
    },
};

export default inventoryService;