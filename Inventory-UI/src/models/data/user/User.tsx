export interface Login
{
    Username: string;
    Password: string;
}

export interface UserRole
{
    Username: string;
    RoleName: string;
}

export interface NewPassword
{
    NewPassword: string;
    OldPassword: string;
}

export interface ResetPassword extends NewPassword {
    Token: string;
}

export interface BasicUserData
{
    Username: string;
    Email: string;
    Role: string;
}

export interface Register
{
    Username: string;
    Email: string;
    Password: string;
    FirstName: string;
    LastName: string;
}

export interface RegisterUserWithRole extends Register
{
    RoleName: string;
}


export interface UpdateUserDetails
{
    FirstName?: string;
    LastName?: string;
    Email?: string;
    FirstLineAddress?: string;
    SecondLineAddress?: string;
    PostCode?: string;
    Gender?: string;
    DOB?: string;
    PhoneNumber?: string;
    ContactNumber?: string;
    Country?: string;
}

export type UserName = string;

export type Token = string;

export type UserEmail = string;