import { useMutation, useQuery } from '@tanstack/react-query';
import userService from '../api/services/userService';

const queryKey = {
  GetUser: "GetUser",
  GetUsers: "GetUsers",
  GetUserById: "GetUserById",
  GetUserDetails: "GetUserDetails",
  AddUser: "AddUser",
  UpdateUser: "UpdateUser",
  RemoveUser: "RemoveUser",
  GetProduct: "GetProduct"
};

export const useAssignUserRole = () => {
  return useMutation({ mutationFn: userService.AssignUserRole });
};

export const useChangePassword = () => {
  return useMutation({ mutationFn: userService.ChangePassword });
};

export const useConfirmation = () => {
  return useMutation({ mutationFn: userService.Confirmation });
};

export const useDisableUser = () => {
  return useMutation({ mutationFn: userService.DisableUser });
};

export const useEnableUser = () => {
  return useMutation({ mutationFn: userService.EnableUser });
};

export const useForgottenPasswordByEmail = () => {
  return useMutation({ mutationFn: userService.ForgottenPasswordByEmail });
};

export const useForgottenPasswordByUsername = () => {
  return useMutation({ mutationFn: userService.ForgottenPasswordByUsername });
};

export const useGetUser = (userIdKey: string) => {
  return useQuery({ queryKey: [queryKey.GetUser, userIdKey], queryFn: userService.GetUser });
};

export const useGetUserDetails = (userIdCacheKey: string) => {
  return useQuery({ queryKey: [queryKey.GetUserDetails, userIdCacheKey], queryFn: userService.GetUserDetails });
};

export const useGetUserDetailsByUser = (userId: string) => {
  return useQuery({ queryKey: [queryKey.GetUserById, userId], queryFn: async () => await userService.GetUserDetailsByUser(userId) });
};

export const useLogin = () => {
  return useMutation({ mutationFn: userService.Login });
};

export const useLogout = () => {
  return useMutation({ mutationFn: userService.Logout });
};

export const useRegister = () => {
  return useMutation({ mutationFn: userService.Register });
};

export const useRegisterUserWithRole = () => {
  return useMutation({ mutationFn: userService.RegisterUserWithRole });
};

export const useResetPassword = () => {
  return useMutation({ mutationFn: userService.ResetPassword });
};