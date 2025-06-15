import formatString from '../../helper/genericHelper';
import { BasicUserData, Login, NewPassword, Register, RegisterUserWithRole, ResetPassword, Token, UpdateUserDetails, UserEmail, UserName, UserRole } from '../../models/data/user/User';
import axiosInstance from '../axios/axiosInstance';

const urlUserPaths = {
    login: '/Login',
    AssignUserRole: '/AssignUserRole',
    ChangePassword: '/ChangePassword',
    Confirmation: '/Confirmation',
    DisableUser: '/Disable/{0}',
    EnableUser: '/Enable/{0}',
    ForgottenPasswordByEmail: '/ForgottenPasswordByEmail/{0}',
    ForgottenPasswordByUsername: '/ForgottenPasswordByUsername/{0}',
    GetUserDetailsByUser: '/GetUserDetailsByUser/{0}',
    GetUsers: '/GetUsers',
    GetUser: '/GetUser',
    GetUserDetails: '/GetUserDetails',
    Login: '/Login',
    Logout: '/Logout',
    Register: '/Register',
    RegisterUserWithRole: '/RegisterUserWithRole',
    ResetPassword: '/ResetPassword',
    Update: '/Update'
}

const userService = {
    AssignUserRole: async (userRole: UserRole) => {
        try {
            const response = await axiosInstance.post(urlUserPaths.AssignUserRole, {
                UserName: userRole.Username,
                roleName: userRole.RoleName
            });
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    ChangePassword: async (newPassword: NewPassword) => {
        try {
            const response = await axiosInstance.post(urlUserPaths.ChangePassword, {
                NewPassword: newPassword.NewPassword,
                OldPassword: newPassword.OldPassword,
        });
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    Confirmation: async (token: Token) => {
        try {
            const response = await axiosInstance.post(formatString(urlUserPaths.Confirmation, token));
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    DisableUser: async (userName: UserName) => {
        try {
            const response = await axiosInstance.get(formatString(urlUserPaths.DisableUser, userName));
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    EnableUser: async (userName: UserName) => {
        try {
            const response = await axiosInstance.get(formatString(urlUserPaths.EnableUser, userName));
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    ForgottenPasswordByEmail: async (userEmail: UserEmail) => {
        try {
            const response = await axiosInstance.get(formatString(urlUserPaths.ForgottenPasswordByEmail, userEmail));
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    ForgottenPasswordByUsername: async (username: UserName) => {
        try {
            const response = await axiosInstance.get(formatString(urlUserPaths.ForgottenPasswordByUsername, username));
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    GetUserDetailsByUser: async (username: UserName) => {
        try {
            const response = await axiosInstance.get(formatString(urlUserPaths.GetUserDetailsByUser, username));
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    GetUsers: async () => {
        try {
            const response = await axiosInstance.get(urlUserPaths.GetUsers);
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    GetUser: async (): Promise<BasicUserData> => {
        try {
            const response = await axiosInstance.get<BasicUserData>(formatString(urlUserPaths.GetUser));
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    GetUserDetails: async () => {
        try {
            const response = await axiosInstance.get(urlUserPaths.GetUserDetails);
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    Login: async (userLogin: Login) => {
        try {
            const response = await axiosInstance.post(urlUserPaths.Login, {
                Username: userLogin.Username,
                Password: userLogin.Password                
            });
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    Logout: async () => {
        try {
            const response = await axiosInstance.post(urlUserPaths.Logout);
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    Register: async (userRegister: Register) => {
        try {
            const response = await axiosInstance.post(urlUserPaths.Register, {
                Username: userRegister.Username,
                Email: userRegister.Email,
                Password: userRegister.Password,
                FirstName: userRegister.FirstName,
                LastName: userRegister.LastName                
            });
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    RegisterUserWithRole: async (registerUserWithRole: RegisterUserWithRole) => {
        try 
        {
            const response = await axiosInstance.post(urlUserPaths.RegisterUserWithRole, {
                Username: registerUserWithRole.Username,
                Email: registerUserWithRole.Email,
                Password: registerUserWithRole.Password,
                FirstName: registerUserWithRole.FirstName,
                LastName: registerUserWithRole.LastName,
                RoleName: registerUserWithRole.RoleName                
            });
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    ResetPassword: async (resetPassword: ResetPassword) => {
        try {
            const response = await axiosInstance.post(urlUserPaths.ResetPassword, {
                Token: resetPassword.Token,
                NewPassword: resetPassword.NewPassword,
                OldPassword: resetPassword.OldPassword                
            });
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    },
    UpdateUser: async (updateUserDetails: UpdateUserDetails) => {
        try {
            const response = await axiosInstance.post(urlUserPaths.Update, {
                FirstName: updateUserDetails.FirstName,
                LastName: updateUserDetails.LastName,
                Email: updateUserDetails.Email,
                FirstLineAddress: updateUserDetails.FirstLineAddress,
                SecondLineAddress: updateUserDetails.SecondLineAddress,
                PostCode: updateUserDetails.PostCode,
                Gender: updateUserDetails.Gender,
                DOB: updateUserDetails.DOB,
                PhoneNumber: updateUserDetails.PhoneNumber,
                ContactNumber: updateUserDetails.ContactNumber,
                Country: updateUserDetails.Country
            });
            return response.data;
        } 
        catch (error) {
            console.error(`Error fetching item with ID`, error);
            throw error;
        }
    }
};

export default userService;