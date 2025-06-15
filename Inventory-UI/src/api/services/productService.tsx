import formatString from '../../helper/genericHelper';
import { ProductId } from '../../models/data/inventory/inventory';
import { Product, UpdateProductDetails } from '../../models/data/product/poduct';
import axiosInstance from '../axios/axiosInstance';

const urlProductPaths = {
    getProductById: '/GetProductById/{0}',
    addProduct: '/AddProduct',
    getProducts: '/GetProducts',
    removeProduct: '/RemoveProduct/{0}',
    UpdateProduct: '/UpdateProduct'
}

const inventoryService = {
    GetProductById: async (productId: ProductId) => {
        try {
            const response = await axiosInstance.get(formatString(urlProductPaths.getProductById, productId));
            return response.data;
        } catch (error) {
            console.error('Error fetching product:', error);
            throw error;
        }
    },
    AddProduct: async (product: Product) => {
        try {
            const response = await axiosInstance.post(urlProductPaths.addProduct, {
                Name: product.Name,
                Description : product.Description,
                ItemQuantity: product.ItemQuantity,
                EnabledPrice: product.EnabledPrice,
                Price: product.Price,
                InventoryQuantity: product.InventoryQuantity,
                Currency: product.Currency
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching product:', error);
            throw error;
        }
    },
    GetProducts: async () => {
        try {
            const response = await axiosInstance.get(urlProductPaths.getProducts);
            return response.data;
        } catch (error) {
            console.error('Error fetching product:', error);
            throw error;
        }
    },
    RemoveProduct: async (productId: ProductId) => {
        try {
            const response = await axiosInstance.get(formatString(urlProductPaths.removeProduct, productId));
            return response.data;
        } catch (error) {
            console.error('Error fetching product:', error);
            throw error;
        }
    },
    UpdateProduct: async (updateProductDetails: UpdateProductDetails) => {
        try {
            const response = await axiosInstance.post(urlProductPaths.UpdateProduct, {
                ProductId: updateProductDetails.ProductId,
                Description: updateProductDetails.Description,
                Price: updateProductDetails.Price,
                ProductName: updateProductDetails.ProductName,
                CurrencyCode: updateProductDetails.CurrencyCode,
                Quantity: updateProductDetails.Quantity,
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching product:', error);
            throw error;
        }
    }
}

export default inventoryService;