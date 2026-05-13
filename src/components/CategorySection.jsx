import React from 'react';
import AppCard from './AppCard';

const CategorySection = ({ category, selectedApps, onToggle }) => {
  return (
    <div className="category-section">
      <h2 className="category-title">{category.name}</h2>
      <div className="app-grid">
        {category.apps.map(app => (
          <AppCard 
            key={app.id} 
            app={app} 
            isSelected={selectedApps.some(selected => selected.id === app.id)}
            onToggle={onToggle}
          />
        ))}
      </div>
    </div>
  );
};

export default CategorySection;
