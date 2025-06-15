import { useMutation, useQuery } from '@tanstack/react-query';
import InventoryService from '../api/services/inventoryService';
import { InventoryItemQuantity } from '../models/data/inventory/inventory';

const queryKey = {
  GetInventoryItems: "GetInventoryItems",
  GetInventoryItem: "GetInventoryItem"
}

export const useAllInventory = () => {
  return useQuery({queryKey: [queryKey.GetInventoryItems], queryFn: InventoryService.getInventory});
};

export const useInventoryItem = (productId: string) => {
  return useQuery({queryKey: [queryKey.GetInventoryItem, productId], queryFn: async() => await InventoryService.getInventoryItemByProductId(productId)});
};

export const useUpdateInventoryItem = () => {
  return useMutation({ mutationFn: InventoryService.updateItemInInventory });
};

export const useAddInventoryItem = () => {
  return useMutation({ mutationFn: InventoryService.UpdateInventoryItemQuantity });
};

export const useDeleteInventoryItem = () => {
  return useMutation({ 
      mutationFn: InventoryService.UpdateInventoryItemQuantity,
      onMutate: (inventoryItemQuantity: InventoryItemQuantity) => {
        return {
          productId: inventoryItemQuantity.ProductId,
          quantity: -inventoryItemQuantity.Quantity
        }
      }
    });
};