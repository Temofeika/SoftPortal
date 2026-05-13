import React from 'react';
import { Download, X, Copy, Check } from 'lucide-react';
import { generatePowerShellScript } from '../utils/ScriptGenerator';

const InstallerModal = ({ selectedApps, onClose }) => {
  const [copied, setCopied] = React.useState(false);
  const script = generatePowerShellScript(selectedApps);

  const handleCopy = () => {
    navigator.clipboard.writeText(script);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const handleDownload = () => {
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
        <h2>Ready to Install?</h2>
        <p>We've generated a PowerShell script to install your {selectedApps.length} selected apps using Winget.</p>
        
        <div className="script-box">
          <pre>{script}</pre>
        </div>

        <div className="modal-actions">
          <button className="btn-secondary" onClick={handleCopy}>
            {copied ? <Check size={18} /> : <Copy size={18} />}
            {copied ? 'Copied!' : 'Copy Script'}
          </button>
          <button className="btn-primary" onClick={handleDownload}>
            <Download size={18} />
            Download .ps1
          </button>
          <button className="btn-secondary" onClick={onClose}>
            <X size={18} />
            Close
          </button>
        </div>
        
        <p style={{ marginTop: '1.5rem', fontSize: '0.8rem', opacity: 0.7 }}>
          Note: You must run the downloaded script as Administrator.
        </p>
      </div>
    </div>
  );
};

export default InstallerModal;
