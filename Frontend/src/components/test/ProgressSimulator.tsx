import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './ProgressSimulator.css';

interface TestData {
  parents: Array<{ id: number; firstName: string; lastName: string; username: string }>;
  children: Array<{ id: number; firstName: string; lastName: string; username: string; parentId: number }>;
  modules: Array<{ id: number; title: string; subject: string }>;
}

const ProgressSimulator: React.FC = () => {
  const [testData, setTestData] = useState<TestData | null>(null);
  const [selectedChild, setSelectedChild] = useState<number>(0);
  const [selectedModule, setSelectedModule] = useState<number>(0);
  const [score, setScore] = useState<number>(85);
  const [timeSpent, setTimeSpent] = useState<number>(25);
  const [attempts, setAttempts] = useState<number>(1);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string>('');

  useEffect(() => {
    loadTestData();
  }, []);

  const loadTestData = async () => {
    try {
      const response = await axios.get('/test/test-data');
      setTestData(response.data);
    } catch (error) {
      console.error('Error loading test data:', error);
    }
  };

  const simulateProgress = async () => {
    if (!selectedChild || !selectedModule) {
      setMessage('Veuillez sélectionner un enfant et un module');
      return;
    }

    setLoading(true);
    setMessage('');

    try {
      const response = await axios.post('/test/simulate-progress', {
        childId: selectedChild,
        moduleId: selectedModule,
        status: 2, // Completed
        score: score,
        timeSpentMinutes: timeSpent,
        attempts: attempts
      });

      setMessage(`✅ ${response.data.message}`);
      
      // Reset form
      setScore(85);
      setTimeSpent(25);
      setAttempts(1);
    } catch (error) {
      setMessage('❌ Erreur lors de la simulation');
      console.error('Error simulating progress:', error);
    } finally {
      setLoading(false);
    }
  };

  if (!testData) {
    return <div className="progress-simulator">Chargement des données de test...</div>;
  }

  return (
    <div className="progress-simulator">
      <h2>Simulateur de Progression</h2>
      <p>Utilisez cet outil pour simuler la progression d'un enfant et tester le tableau de bord parent.</p>

      <div className="simulator-form">
        <div className="form-group">
          <label>Enfant:</label>
          <select 
            value={selectedChild} 
            onChange={(e) => setSelectedChild(Number(e.target.value))}
          >
            <option value={0}>Sélectionner un enfant</option>
            {testData.children.map(child => (
              <option key={child.id} value={child.id}>
                {child.firstName} {child.lastName} ({child.username})
              </option>
            ))}
          </select>
        </div>

        <div className="form-group">
          <label>Module:</label>
          <select 
            value={selectedModule} 
            onChange={(e) => setSelectedModule(Number(e.target.value))}
          >
            <option value={0}>Sélectionner un module</option>
            {testData.modules.map(module => (
              <option key={module.id} value={module.id}>
                {module.title} ({module.subject})
              </option>
            ))}
          </select>
        </div>

        <div className="form-group">
          <label>Score (%):</label>
          <input 
            type="range" 
            min="0" 
            max="100" 
            value={score} 
            onChange={(e) => setScore(Number(e.target.value))}
          />
          <span>{score}%</span>
        </div>

        <div className="form-group">
          <label>Temps passé (minutes):</label>
          <input 
            type="number" 
            min="1" 
            max="120" 
            value={timeSpent} 
            onChange={(e) => setTimeSpent(Number(e.target.value))}
          />
        </div>

        <div className="form-group">
          <label>Nombre de tentatives:</label>
          <input 
            type="number" 
            min="1" 
            max="10" 
            value={attempts} 
            onChange={(e) => setAttempts(Number(e.target.value))}
          />
        </div>

        <button 
          onClick={simulateProgress} 
          disabled={loading || !selectedChild || !selectedModule}
          className="btn btn-primary"
        >
          {loading ? 'Simulation...' : 'Simuler la progression'}
        </button>

        {message && (
          <div className={`message ${message.includes('✅') ? 'success' : 'error'}`}>
            {message}
          </div>
        )}
      </div>

      <div className="test-info">
        <h3>Informations de test</h3>
        <div className="info-grid">
          <div>
            <h4>Parents ({testData.parents.length})</h4>
            <ul>
              {testData.parents.map(parent => (
                <li key={parent.id}>
                  {parent.firstName} {parent.lastName} ({parent.username})
                </li>
              ))}
            </ul>
          </div>
          <div>
            <h4>Enfants ({testData.children.length})</h4>
            <ul>
              {testData.children.map(child => (
                <li key={child.id}>
                  {child.firstName} {child.lastName} ({child.username})
                  <br />
                  <small>Parent ID: {child.parentId}</small>
                </li>
              ))}
            </ul>
          </div>
          <div>
            <h4>Modules ({testData.modules.length})</h4>
            <ul>
              {testData.modules.map(module => (
                <li key={module.id}>
                  {module.title} ({module.subject})
                </li>
              ))}
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProgressSimulator; 