
export type Login = {
    username: string;
    password: string;
    rememberMe: boolean;
    error?: string;
    isLoading?: boolean;
    isAuthenticated?: boolean;
    redirectTo?: string;
}