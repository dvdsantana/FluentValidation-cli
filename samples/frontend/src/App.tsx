import { useState } from 'react';
import './App.css';
import { UserForm } from './components/UserForm';
import { ProductForm } from './components/ProductForm';

type Tab = 'user' | 'product';

function App() {
  const [activeTab, setActiveTab] = useState<Tab>('user');

  return (
    <div className="app">
      <header className="app-header">
        <h1>🚀 FluentValidation Code Generator Demo</h1>
        <p>
          Demonstrating <strong>identical validation logic</strong> on both frontend (TypeScript)
          and backend (C#)
        </p>
        <p className="tech-stack">
          <span className="badge">React</span>
          <span className="badge">TypeScript</span>
          <span className="badge">fluentvalidation-ts</span>
          <span className="badge">ASP.NET Core</span>
          <span className="badge">FluentValidation</span>
        </p>
      </header>

      <div className="tabs">
        <button
          className={`tab ${activeTab === 'user' ? 'active' : ''}`}
          onClick={() => setActiveTab('user')}
        >
          User Registration
        </button>
        <button
          className={`tab ${activeTab === 'product' ? 'active' : ''}`}
          onClick={() => setActiveTab('product')}
        >
          Product Creation
        </button>
      </div>

      <main className="app-main">
        {activeTab === 'user' && <UserForm />}
        {activeTab === 'product' && <ProductForm />}

        <div className="info-box">
          <h3>✨ How It Works</h3>
          <ol>
            <li>
              <strong>Define once:</strong> Validation rules are defined in JSON files
            </li>
            <li>
              <strong>Generate C#:</strong> Run <code>fv-generator</code> to create FluentValidation
              classes
            </li>
            <li>
              <strong>Generate TypeScript:</strong> Run <code>fv-ts-generator</code> to create
              fluentvalidation-ts classes
            </li>
            <li>
              <strong>Use everywhere:</strong> Frontend and backend have identical validation logic!
            </li>
          </ol>

          <div className="demo-instructions">
            <h4>Try these test cases:</h4>
            <ul>
              <li>
                <strong>User:</strong> Email must be valid, Age 18-120, Name 2-100 characters
              </li>
              <li>
                <strong>Product:</strong> SKU must match XXX-9999 format, Price 0-100,000
              </li>
            </ul>
          </div>
        </div>
      </main>

      <footer className="app-footer">
        <p>
          Backend API running on{' '}
          <a href="https://localhost:7139/swagger" target="_blank" rel="noopener noreferrer">
            https://localhost:7139/swagger
          </a>
        </p>
      </footer>
    </div>
  );
}

export default App;
