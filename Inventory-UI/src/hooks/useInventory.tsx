import { useQuery } from '@tanstack/react-query';
import InventoryService from '../services/inventoryService';
import { InventoryItem } from '../models/data/InventoryItem';

export const useAllInventory = () => {
  return useQuery({queryKey: ["GetAllInventoryItems"], queryFn: InventoryService.getAllItems});
};

export const useInventoryItem = (ItemId: string) => {
  return useQuery({queryKey: ["GetInventoryItem", ItemId], queryFn: async() => await InventoryService.getItemById(ItemId)});
};

export const useCreateInventoryItem = (ItemData: InventoryItem) => {
  return useQuery({queryKey: ["CreateInventoryItem", ItemData], queryFn: async() => await InventoryService.createItem(ItemData)});
};

export const useDeleteInventoryItem = (ItemId: string) => {
  return useQuery({queryKey: ["DeleteItem", ItemId], queryFn: async() => await InventoryService.deleteItem(ItemId)});
};

export const useUpdateInventoryItem = (ItemId: string, ItemData: InventoryItem) => {
  return useQuery({queryKey: ["UpdateItem", ItemId, ItemData], queryFn: async() => await InventoryService.updateItem(ItemId, ItemData)});
};