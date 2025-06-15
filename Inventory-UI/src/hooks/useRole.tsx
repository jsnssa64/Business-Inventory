import { useMutation, useQuery } from '@tanstack/react-query';
import productService from '../api/services/productService';

const queryKey = {
  GetProducts: "GetProducts",
  GetProduct: "GetProduct"
}

export const useAllProduct = () => {
  return useQuery({queryKey: [queryKey.GetProducts], queryFn: productService.GetProducts});
};

export const useProduct = (productId: string) => {
  return useQuery({queryKey: [queryKey.GetProduct, productId], queryFn: async() => await productService.GetProductById(productId)});
};

export const useNewProduct = () => {
  return useMutation({ mutationFn: productService.AddProduct });
};

export const useUpdateProduct = () => {
  return useMutation({ mutationFn: productService.UpdateProduct });
};

export const useRemoveProduct = () => {
  return useMutation({ mutationFn: productService.RemoveProduct });
};