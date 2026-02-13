export interface IUser {
    email: string;
    password: string;
    code: string;
    name: string;
    isAuthenticated?: boolean;
  }