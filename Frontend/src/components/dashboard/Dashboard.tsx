import React, { useState, useEffect } from 'react';
import { useAuth } from '../../contexts/AuthContext';
import { User } from '../../types';
import { authService } from '../../services/authService';

const Dashboard: React.FC = () => {
  const { user, logout } = useAuth();
  const [children, setChildren] = useState<User[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (user && user.role === 2) { // Parent role
      loadChildren();
    }
  }, [user]);

  const loadChildren = async () => {
    if (!user) return;
    
    setLoading(true);
    try {
      const childrenData = await authService.getChildren(user.id);
      setChildren(childrenData);
    } catch (error) {
      console.error('Error loading children:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    logout();
  };

  const getRoleDisplayName = (role: number) => {
    switch (role) {
      case 1: return 'Enfant';
      case 2: return 'Parent';
      case 3: return 'Enseignant';
      case 4: return 'Administrateur';
      default: return 'Utilisateur';
    }
  };

  const getLevelDisplayName = (level: number) => {
    switch (level) {
      case 1: return 'Débutant';
      case 2: return 'Intermédiaire';
      case 3: return 'Avancé';
      default: return 'Non défini';
    }
  };

  if (!user) {
    return <div>Chargement...</div>;
  }

  return (
    <div className="dashboard-container">
      <div className="dashboard-header">
        <h1>Bienvenue, {user.firstName} {user.lastName} !</h1>
        <p>Rôle: {getRoleDisplayName(user.role)} | Niveau: {getLevelDisplayName(user.level)}</p>
        <button onClick={handleLogout} className="btn btn-primary" style={{ marginTop: '10px' }}>
          Se déconnecter
        </button>
      </div>

      <div className="dashboard-content">
        <div className="dashboard-card">
          <h3>Informations du profil</h3>
          <p><strong>Nom d'utilisateur:</strong> {user.username}</p>
          <p><strong>Email:</strong> {user.email}</p>
          <p><strong>Âge:</strong> {user.age} ans</p>
          <p><strong>Membre depuis:</strong> {new Date(user.createdAt).toLocaleDateString('fr-FR')}</p>
          {user.lastLoginAt && (
            <p><strong>Dernière connexion:</strong> {new Date(user.lastLoginAt).toLocaleDateString('fr-FR')}</p>
          )}
        </div>

        {user.role === 2 && ( // Parent role
          <div className="dashboard-card">
            <h3>Mes enfants</h3>
            {loading ? (
              <p>Chargement des enfants...</p>
            ) : children.length > 0 ? (
              <div>
                {children.map((child) => (
                  <div key={child.id} style={{ 
                    border: '1px solid #ddd', 
                    padding: '10px', 
                    margin: '10px 0', 
                    borderRadius: '5px' 
                  }}>
                    <p><strong>{child.firstName} {child.lastName}</strong></p>
                    <p>Nom d'utilisateur: {child.username}</p>
                    <p>Âge: {child.age} ans</p>
                    <p>Niveau: {getLevelDisplayName(child.level)}</p>
                  </div>
                ))}
              </div>
            ) : (
              <p>Aucun enfant enregistré</p>
            )}
            <div style={{ marginTop: '15px' }}>
              <a href="/register-child" className="btn btn-success">
                Ajouter un enfant
              </a>
            </div>
          </div>
        )}

        <div className="dashboard-card">
          <h3>Actions rapides</h3>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
            <button className="btn btn-primary">
              Commencer un cours
            </button>
            <button className="btn btn-primary">
              Voir ma progression
            </button>
            <button className="btn btn-primary">
              Paramètres du compte
            </button>
          </div>
        </div>

        <div className="dashboard-card">
          <h3>Statistiques</h3>
          <p>Fonctionnalité à venir...</p>
          <p>Ici vous pourrez voir vos statistiques d'apprentissage, vos progrès et vos réalisations.</p>
        </div>
      </div>
    </div>
  );
};

export default Dashboard; 