import formatString from '../../helper/genericHelper';
import { ProductId } from '../../models/data/inventory/inventory';
import axiosInstance from '../axios/axiosInstance';

const urlRolePaths = {
    getRoles: '/GetRoles',
    getDefaultRole: '/GetDefaultRole'
}

const inventoryService = {
    getRoles: async () => {
        try {
            const response = await axiosInstance.get(urlRolePaths.getRoles);
            return response.data;
        } catch (error) {
            console.error('Error fetching inventory items:', error);
            throw error;
        }
    },
    getDefaultRole: async () => {
        try {
            const response = await axiosInstance.get(urlRolePaths.getDefaultRole);
            return response.data;
        } catch (error) {
            console.error('Error fetching inventory items:', error);
            throw error;
        }
    }
}

export default inventoryService;