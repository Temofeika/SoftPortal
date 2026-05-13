import React from 'react';
import * as Icons from 'lucide-react';

const AppCard = ({ app, isSelected, onToggle }) => {
  const IconComponent = Icons[app.icon] || Icons.Package;

  return (
    <div 
      className={`app-card ${isSelected ? 'selected' : ''}`} 
      onClick={() => onToggle(app)}
    >
      <div className="app-icon">
        <IconComponent size={24} />
      </div>
      <div className="app-name">{app.name}</div>
      <div className="checkbox">
        {isSelected && <Icons.Check size={14} color="white" />}
      </div>
    </div>
  );
};

export default AppCard;
