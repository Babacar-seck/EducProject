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

// Progress tracking types
export interface Progress {
  id: number;
  userId: number;
  userName: string;
  moduleId: number;
  moduleTitle: string;
  subject: string;
  status: ProgressStatus;
  score: number;
  maxScore: number;
  percentage: number;
  startedAt?: string;
  completedAt?: string;
  timeSpentMinutes: number;
  attempts: number;
  createdAt: string;
  updatedAt: string;
}

export enum ProgressStatus {
  NotStarted = 0,
  InProgress = 1,
  Completed = 2,
  Failed = 3,
  Paused = 4
}

export interface Badge {
  id: number;
  name: string;
  description: string;
  type: string;
  iconPath: string;
  color: string;
  earnedAt: string;
  isNotified: boolean;
}

export interface Notification {
  id: number;
  title: string;
  message: string;
  type: string;
  priority: string;
  isRead: boolean;
  createdAt: string;
  readAt?: string;
  relatedUserName?: string;
}

export interface ChildProgressSummary {
  childId: number;
  childName: string;
  totalModules: number;
  completedModules: number;
  inProgressModules: number;
  averageScore: number;
  totalTimeSpentMinutes: number;
  totalBadges: number;
  recentProgress: Progress[];
  recentBadges: Badge[];
}

export interface CreateProgressRequest {
  userId: number;
  moduleId: number;
  status: ProgressStatus;
  score: number;
  timeSpentMinutes: number;
}

export interface UpdateProgressRequest {
  status: ProgressStatus;
  score: number;
  timeSpentMinutes: number;
  attempts: number;
} 