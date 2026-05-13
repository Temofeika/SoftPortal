import React from 'react';
import { Download, X, Copy, Check, Terminal } from 'lucide-react';
import { generatePowerShellScript } from '../utils/ScriptGenerator';

const InstallerModal = ({ selectedApps, onClose }) => {
  const [copied, setCopied] = React.useState(false);
  const script = generatePowerShellScript(selectedApps);

  const handleCopy = () => {
    navigator.clipboard.writeText(script);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const handleDownloadExe = async () => {
    try {
      const response = await fetch(`${import.meta.env.BASE_URL}SoftPortal.exe`);
      const blob = await response.blob();
      
      // Generate filename: SoftPortal_ID1_ID2.exe
      const ids = selectedApps.map(app => app.id).join('_');
      const fileName = `SoftPortal_${ids}.exe`;
      
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      link.download = fileName;
      link.click();
    } catch (error) {
      console.error('Failed to download EXE:', error);
      alert('Failed to generate EXE. Please use the PowerShell script instead.');
    }
  };

  const handleDownloadPs1 = () => {
    const element = document.createElement("a");
    const file = new Blob([script], {type: 'text/plain'});
    element.href = URL.createObjectURL(file);
    element.download = "install_apps.ps1";
    document.body.appendChild(element);
    element.click();
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={e => e.stopPropagation()}>
        <h2>Get Your Installer</h2>
        <p>We've prepared a custom <b>.exe</b> installer for your {selectedApps.length} selected apps.</p>
        
        <div style={{ margin: '2rem 0', padding: '1.5rem', background: 'rgba(0, 112, 243, 0.05)', borderRadius: '16px', border: '1px solid var(--accent-color)' }}>
          <h3 style={{ marginBottom: '0.5rem', color: 'var(--accent-color)' }}>Recommended</h3>
          <p style={{ fontSize: '0.9rem', marginBottom: '1.5rem' }}>The EXE installer automatically runs Winget commands for you.</p>
          <button className="btn-primary" onClick={handleDownloadExe} style={{ width: '100%', justifyContent: 'center' }}>
            <Download size={18} />
            Download .exe Installer
          </button>
        </div>

        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', margin: '1rem 0' }}>
          <div style={{ flex: 1, height: '1px', background: 'var(--card-border)' }}></div>
          <span style={{ color: 'var(--text-secondary)', fontSize: '0.8rem' }}>OR USE SCRIPT</span>
          <div style={{ flex: 1, height: '1px', background: 'var(--card-border)' }}></div>
        </div>

        <div className="modal-actions">
          <button className="btn-secondary" onClick={handleDownloadPs1}>
            <Terminal size={18} />
            Download .ps1
          </button>
          <button className="btn-secondary" onClick={onClose}>
            <X size={18} />
            Close
          </button>
        </div>
        
        <p style={{ marginTop: '1.5rem', fontSize: '0.8rem', opacity: 0.7 }}>
          Note: Windows might show a warning because the EXE is unsigned.
        </p>
      </div>
    </div>
  );
};

export default InstallerModal;
