import React, { useState, useEffect } from 'react';
import { useAuth } from '../../contexts/AuthContext';
import { progressService } from '../../services/progressService';
import { ChildProgressSummary, Notification, Progress, Badge } from '../../types';
import './ParentDashboard.css';

const ParentDashboard: React.FC = () => {
  const { user } = useAuth();
  const [childrenProgress, setChildrenProgress] = useState<ChildProgressSummary[]>([]);
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [selectedChild, setSelectedChild] = useState<ChildProgressSummary | null>(null);

  useEffect(() => {
    if (user) {
      loadDashboardData();
    }
  }, [user]);

  const loadDashboardData = async () => {
    if (!user) return;
    
    setLoading(true);
    try {
      const [childrenData, notificationsData, unreadCountData] = await Promise.all([
        progressService.getChildrenProgressSummary(user.id),
        progressService.getUserNotifications(user.id),
        progressService.getUnreadNotificationsCount(user.id)
      ]);

      setChildrenProgress(childrenData);
      setNotifications(notificationsData);
      setUnreadCount(unreadCountData);
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleMarkAllAsRead = async () => {
    if (!user) return;
    
    const success = await progressService.markAllNotificationsAsRead(user.id);
    if (success) {
      setNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
      setUnreadCount(0);
    }
  };

  const handleMarkAsRead = async (notificationId: number) => {
    const success = await progressService.markNotificationAsRead(notificationId);
    if (success) {
      setNotifications(prev => 
        prev.map(n => n.id === notificationId ? { ...n, isRead: true } : n)
      );
      setUnreadCount(prev => Math.max(0, prev - 1));
    }
  };

  const getProgressStatusColor = (status: number) => {
    switch (status) {
      case 0: return '#6c757d'; // NotStarted
      case 1: return '#ffc107'; // InProgress
      case 2: return '#28a745'; // Completed
      case 3: return '#dc3545'; // Failed
      case 4: return '#17a2b8'; // Paused
      default: return '#6c757d';
    }
  };

  const getProgressStatusText = (status: number) => {
    switch (status) {
      case 0: return 'Non commencé';
      case 1: return 'En cours';
      case 2: return 'Terminé';
      case 3: return 'Échoué';
      case 4: return 'En pause';
      default: return 'Inconnu';
    }
  };

  const formatTime = (minutes: number) => {
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return hours > 0 ? `${hours}h ${mins}min` : `${mins}min`;
  };

  if (loading) {
    return (
      <div className="parent-dashboard">
        <div className="loading">Chargement du tableau de bord...</div>
      </div>
    );
  }

  return (
    <div className="parent-dashboard">
      <div className="dashboard-header">
        <h1>Tableau de Bord Parent</h1>
        <p>Suivi de la progression de vos enfants</p>
        
        {unreadCount > 0 && (
          <div className="notifications-badge">
            <span>{unreadCount}</span>
            <button onClick={handleMarkAllAsRead} className="btn btn-primary">
              Marquer tout comme lu
            </button>
          </div>
        )}
      </div>

      <div className="dashboard-content">
        {/* Children Progress Overview */}
        <div className="children-overview">
          <h2>Progression des Enfants</h2>
          <div className="children-grid">
            {childrenProgress.map((child) => (
              <div 
                key={child.childId} 
                className={`child-card ${selectedChild?.childId === child.childId ? 'selected' : ''}`}
                onClick={() => setSelectedChild(child)}
              >
                <div className="child-header">
                  <h3>{child.childName}</h3>
                  <div className="child-stats">
                    <span className="stat">
                      <strong>{child.completedModules}</strong> / {child.totalModules} modules
                    </span>
                    <span className="stat">
                      Score moyen: <strong>{child.averageScore.toFixed(1)}%</strong>
                    </span>
                  </div>
                </div>
                
                <div className="progress-bar">
                  <div 
                    className="progress-fill" 
                    style={{ width: `${child.totalModules > 0 ? (child.completedModules / child.totalModules) * 100 : 0}%` }}
                  ></div>
                </div>
                
                <div className="child-details">
                  <div className="detail">
                    <span>⏱️ Temps total:</span>
                    <span>{formatTime(child.totalTimeSpentMinutes)}</span>
                  </div>
                  <div className="detail">
                    <span>🏆 Badges:</span>
                    <span>{child.totalBadges}</span>
                  </div>
                  <div className="detail">
                    <span>📚 En cours:</span>
                    <span>{child.inProgressModules}</span>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Selected Child Details */}
        {selectedChild && (
          <div className="child-details-section">
            <h2>Détails - {selectedChild.childName}</h2>
            
            <div className="details-grid">
              {/* Recent Progress */}
              <div className="detail-card">
                <h3>Progression Récente</h3>
                <div className="progress-list">
                  {selectedChild.recentProgress.length > 0 ? (
                    selectedChild.recentProgress.map((progress) => (
                      <div key={progress.id} className="progress-item">
                        <div className="progress-info">
                          <h4>{progress.moduleTitle}</h4>
                          <p>{progress.subject}</p>
                        </div>
                        <div className="progress-status">
                          <span 
                            className="status-badge"
                            style={{ backgroundColor: getProgressStatusColor(progress.status) }}
                          >
                            {getProgressStatusText(progress.status)}
                          </span>
                          {progress.status === 2 && (
                            <span className="score">{progress.percentage.toFixed(0)}%</span>
                          )}
                        </div>
                        <div className="progress-meta">
                          <span>⏱️ {formatTime(progress.timeSpentMinutes)}</span>
                          <span>🔄 {progress.attempts} tentatives</span>
                        </div>
                      </div>
                    ))
                  ) : (
                    <p>Aucune progression récente</p>
                  )}
                </div>
              </div>

              {/* Recent Badges */}
              <div className="detail-card">
                <h3>Badges Récents</h3>
                <div className="badges-grid">
                  {selectedChild.recentBadges.length > 0 ? (
                    selectedChild.recentBadges.map((badge) => (
                      <div key={badge.id} className="badge-item" style={{ borderColor: badge.color }}>
                        <div className="badge-icon" style={{ color: badge.color }}>
                          {badge.iconPath}
                        </div>
                        <div className="badge-info">
                          <h4>{badge.name}</h4>
                          <p>{badge.description}</p>
                          <small>{new Date(badge.earnedAt).toLocaleDateString('fr-FR')}</small>
                        </div>
                      </div>
                    ))
                  ) : (
                    <p>Aucun badge récent</p>
                  )}
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Notifications */}
        <div className="notifications-section">
          <h2>Notifications</h2>
          <div className="notifications-list">
            {notifications.length > 0 ? (
              notifications.map((notification) => (
                <div 
                  key={notification.id} 
                  className={`notification-item ${!notification.isRead ? 'unread' : ''}`}
                  onClick={() => !notification.isRead && handleMarkAsRead(notification.id)}
                >
                  <div className="notification-header">
                    <h4>{notification.title}</h4>
                    <span className="notification-time">
                      {new Date(notification.createdAt).toLocaleDateString('fr-FR')}
                    </span>
                  </div>
                  <p>{notification.message}</p>
                  {!notification.isRead && (
                    <span className="unread-indicator">●</span>
                  )}
                </div>
              ))
            ) : (
              <p>Aucune notification</p>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ParentDashboard; 