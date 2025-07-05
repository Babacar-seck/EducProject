export interface User {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  age: number;
  role: UserRole;
  level: UserLevel;
  createdAt: string;
  lastLoginAt?: string;
  parentId?: number;
  children: User[];
}

export enum UserRole {
  Child = 1,
  Parent = 2,
  Teacher = 3,
  Admin = 4
}

export enum UserLevel {
  Beginner = 1,
  Intermediate = 2,
  Advanced = 3
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  role: UserRole;
  level?: UserLevel;
  parentId?: number;
}

export interface RegisterChildRequest {
  username: string;
  password: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  parentId: number;
}

export interface AuthResponse {
  token: string;
  user: User;
  expiresAt: string;
}

export interface ApiResponse<T> {
  data?: T;
  message?: string;
  success: boolean;
} 