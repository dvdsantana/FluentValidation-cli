import { useState } from 'react';
import './App.css';
import { UserValidator } from './validators/UserValidator';
import type { User } from './validators/UserValidator';

function App() {
  const [user, setUser] = useState<User>({
    username: '',
    email: '',
    age: 0
  });
  const [errors, setErrors] = useState<any>({});

  const validator = new UserValidator();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setUser(prev => ({
      ...prev,
      [name]: name === 'age' ? Number(value) : value
    }));
  };

  const validate = () => {
    const result = validator.validate(user);
    setErrors(result);
  };

  return (
    <div className="card">
      <h1>User Validation</h1>
      <div style={{ display: 'flex', flexDirection: 'column', gap: '10px', textAlign: 'left' }}>
        <div>
          <label>Username: </label>
          <input name="username" value={user.username} onChange={handleChange} />
          {errors.username && <span style={{ color: 'red' }}>{errors.username}</span>}
        </div>
        <div>
          <label>Email: </label>
          <input name="email" value={user.email} onChange={handleChange} />
          {errors.email && <span style={{ color: 'red' }}>{errors.email}</span>}
        </div>
        <div>
          <label>Age: </label>
          <input name="age" type="number" value={user.age} onChange={handleChange} />
          {errors.age && <span style={{ color: 'red' }}>{errors.age}</span>}
        </div>
        <button onClick={validate}>Validate</button>
      </div>
    </div>
  );
}

export default App;
