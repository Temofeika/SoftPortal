import React, { useState } from 'react';
import { APP_CATEGORIES } from './data/apps';
import CategorySection from './components/CategorySection';
import InstallerModal from './components/InstallerModal';
import { Package, ChevronRight } from 'lucide-react';

function App() {
  const [selectedApps, setSelectedApps] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const toggleApp = (app) => {
    setSelectedApps(prev => 
      prev.some(a => a.id === app.id)
        ? prev.filter(a => a.id !== app.id)
        : [...prev, app]
    );
  };

  return (
    <div className="App">
      <header>
        <div className="container">
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '0.5rem', marginBottom: '1rem' }}>
            <Package color="#0070f3" size={32} />
            <span style={{ fontWeight: 700, fontSize: '1.2rem', letterSpacing: '-0.5px' }}>SoftPortal</span>
          </div>
          <h1>Install everything at once.</h1>
          <p className="subtitle">
            Select the applications you need and get a custom PowerShell script to install them all automatically using Winget.
          </p>
        </div>
      </header>

      <main className="container categories-container">
        {APP_CATEGORIES.map(category => (
          <CategorySection 
            key={category.name} 
            category={category} 
            selectedApps={selectedApps}
            onToggle={toggleApp}
          />
        ))}
      </main>

      {selectedApps.length > 0 && (
        <div className="action-bar">
          <div className="selection-count">
            <strong>{selectedApps.length}</strong> apps selected
          </div>
          <button className="btn-primary" onClick={() => setIsModalOpen(true)}>
            Get My Apps <ChevronRight size={18} />
          </button>
        </div>
      )}

      {isModalOpen && (
        <InstallerModal 
          selectedApps={selectedApps} 
          onClose={() => setIsModalOpen(false)} 
        />
      )}

      <footer style={{ padding: '4rem 0', textAlign: 'center', borderTop: '1px solid var(--card-border)', marginTop: 'auto' }}>
        <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem' }}>
          Built with React & Winget. No installers, no junkware, no hassle.
        </p>
      </footer>
    </div>
  );
}

export default App;
