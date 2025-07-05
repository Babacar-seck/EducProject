import axios from 'axios';
import { 
  Progress, 
  Badge, 
  Notification, 
  ChildProgressSummary, 
  CreateProgressRequest, 
  UpdateProgressRequest 
} from '../types';

export const progressService = {
  // Progress management
  async getUserProgress(userId: number): Promise<Progress[]> {
    try {
      const response = await axios.get<Progress[]>(`/progress/user/${userId}`);
      return response.data;
    } catch (error) {
      console.error('Get user progress failed:', error);
      return [];
    }
  },

  async getParentChildrenProgress(parentId: number): Promise<Progress[]> {
    try {
      const response = await axios.get<Progress[]>(`/progress/parent/${parentId}`);
      return response.data;
    } catch (error) {
      console.error('Get parent children progress failed:', error);
      return [];
    }
  },

  async getChildProgressSummary(childId: number): Promise<ChildProgressSummary | null> {
    try {
      const response = await axios.get<ChildProgressSummary>(`/progress/child-summary/${childId}`);
      return response.data;
    } catch (error) {
      console.error('Get child progress summary failed:', error);
      return null;
    }
  },

  async getChildrenProgressSummary(parentId: number): Promise<ChildProgressSummary[]> {
    try {
      const response = await axios.get<ChildProgressSummary[]>(`/progress/children-summary/${parentId}`);
      return response.data;
    } catch (error) {
      console.error('Get children progress summary failed:', error);
      return [];
    }
  },

  async createProgress(createDto: CreateProgressRequest): Promise<Progress | null> {
    try {
      const response = await axios.post<Progress>('/progress', createDto);
      return response.data;
    } catch (error) {
      console.error('Create progress failed:', error);
      return null;
    }
  },

  async updateProgress(id: number, updateDto: UpdateProgressRequest): Promise<Progress | null> {
    try {
      const response = await axios.put<Progress>(`/progress/${id}`, updateDto);
      return response.data;
    } catch (error) {
      console.error('Update progress failed:', error);
      return null;
    }
  },

  // Badges
  async getUserBadges(userId: number): Promise<Badge[]> {
    try {
      const response = await axios.get<Badge[]>(`/progress/badges/${userId}`);
      return response.data;
    } catch (error) {
      console.error('Get user badges failed:', error);
      return [];
    }
  },

  // Notifications
  async getUserNotifications(userId: number): Promise<Notification[]> {
    try {
      const response = await axios.get<Notification[]>(`/progress/notifications/${userId}`);
      return response.data;
    } catch (error) {
      console.error('Get user notifications failed:', error);
      return [];
    }
  },

  async markNotificationAsRead(notificationId: number): Promise<boolean> {
    try {
      await axios.put(`/progress/notifications/${notificationId}/read`);
      return true;
    } catch (error) {
      console.error('Mark notification as read failed:', error);
      return false;
    }
  },

  async markAllNotificationsAsRead(userId: number): Promise<boolean> {
    try {
      await axios.put(`/progress/notifications/${userId}/read-all`);
      return true;
    } catch (error) {
      console.error('Mark all notifications as read failed:', error);
      return false;
    }
  },

  async getUnreadNotificationsCount(userId: number): Promise<number> {
    try {
      const response = await axios.get<number>(`/progress/notifications/${userId}/unread-count`);
      return response.data;
    } catch (error) {
      console.error('Get unread notifications count failed:', error);
      return 0;
    }
  }
}; 