import axios from 'axios';
import { LoginRequest, RegisterRequest, RegisterChildRequest, AuthResponse, User } from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7001/api';

// Configure axios defaults
axios.defaults.baseURL = API_BASE_URL;

// Add request interceptor to include token
axios.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Add response interceptor to handle errors
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const authService = {
  async login(credentials: LoginRequest): Promise<AuthResponse | null> {
    try {
      const response = await axios.post<AuthResponse>('/auth/login', credentials);
      return response.data;
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  },

  async register(userData: RegisterRequest): Promise<User | null> {
    try {
      const response = await axios.post<User>('/auth/register', userData);
      return response.data;
    } catch (error) {
      console.error('Registration failed:', error);
      throw error;
    }
  },

  async registerChild(childData: RegisterChildRequest): Promise<User | null> {
    try {
      const response = await axios.post<User>('/auth/register-child', childData);
      return response.data;
    } catch (error) {
      console.error('Child registration failed:', error);
      throw error;
    }
  },

  async validateToken(): Promise<boolean> {
    try {
      await axios.get('/auth/validate');
      return true;
    } catch (error) {
      return false;
    }
  },

  async getUser(id: number): Promise<User | null> {
    try {
      const response = await axios.get<User>(`/users/${id}`);
      return response.data;
    } catch (error) {
      console.error('Get user failed:', error);
      return null;
    }
  },

  async getChildren(parentId: number): Promise<User[]> {
    try {
      const response = await axios.get<User[]>(`/users/children/${parentId}`);
      return response.data;
    } catch (error) {
      console.error('Get children failed:', error);
      return [];
    }
  }
}; 