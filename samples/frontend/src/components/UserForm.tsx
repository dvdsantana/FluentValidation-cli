import { useState, FormEvent } from 'react';
import { UserValidator } from '../validators/UserValidator';
import type { User } from '../validators/UserValidator';
import './UserForm.css';

const userValidator = new UserValidator();

export function UserForm() {
  const [formData, setFormData] = useState<Partial<User>>({
    email: '',
    age: 0,
    name: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitStatus, setSubmitStatus] = useState<'idle' | 'success' | 'error'>('idle');

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setSubmitStatus('idle');

    // Validate using the generated validator
    const validationErrors = userValidator.validate(formData as User);

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      setSubmitStatus('error');
      return;
    }

    // Clear errors if validation passes
    setErrors({});

    // Submit to backend API
    try {
      const response = await fetch('https://localhost:7139/api/users', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });

      if (response.ok) {
        const result = await response.json();
        console.log('User created:', result);
        setSubmitStatus('success');
        // Reset form
        setFormData({ email: '', age: 0, name: '' });
      } else {
        const error = await response.json();
        console.error('Server validation errors:', error);
        setSubmitStatus('error');
      }
    } catch (error) {
      console.error('Network error:', error);
      setSubmitStatus('error');
    }
  };

  const handleChange = (field: keyof User, value: string | number) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
    // Clear error for this field when user starts typing
    if (errors[field]) {
      setErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  return (
    <div className="form-container">
      <h2>Create User</h2>
      <p className="form-subtitle">Validation powered by generated fluentvalidation-ts</p>

      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="name">Name:</label>
          <input
            id="name"
            type="text"
            value={formData.name || ''}
            onChange={(e) => handleChange('name', e.target.value)}
            className={errors.name ? 'error' : ''}
          />
          {errors.name && <span className="error-message">{errors.name}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="email">Email:</label>
          <input
            id="email"
            type="text"
            value={formData.email || ''}
            onChange={(e) => handleChange('email', e.target.value)}
            className={errors.email ? 'error' : ''}
          />
          {errors.email && <span className="error-message">{errors.email}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="age">Age:</label>
          <input
            id="age"
            type="number"
            value={formData.age || ''}
            onChange={(e) => handleChange('age', parseInt(e.target.value) || 0)}
            className={errors.age ? 'error' : ''}
          />
          {errors.age && <span className="error-message">{errors.age}</span>}
        </div>

        <button type="submit" className="submit-btn">
          Create User
        </button>

        {submitStatus === 'success' && (
          <div className="success-message">✓ User created successfully!</div>
        )}
        {submitStatus === 'error' && (
          <div className="error-message">✗ Please fix validation errors</div>
        )}
      </form>
    </div>
  );
}
