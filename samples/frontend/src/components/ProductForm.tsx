import { useState, FormEvent } from 'react';
import { ProductValidator } from '../validators/ProductValidator';
import type { Product } from '../validators/ProductValidator';
import './UserForm.css';

const productValidator = new ProductValidator();

export function ProductForm() {
  const [formData, setFormData] = useState<Partial<Product>>({
    name: '',
    price: 0,
    sku: '',
    description: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitStatus, setSubmitStatus] = useState<'idle' | 'success' | 'error'>('idle');

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setSubmitStatus('idle');

    // Validate using the generated validator
    const validationErrors = productValidator.validate(formData as Product);

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      setSubmitStatus('error');
      return;
    }

    setErrors({});

    // Submit to backend API
    try {
      const response = await fetch('https://localhost:7139/api/products', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });

      if (response.ok) {
        const result = await response.json();
        console.log('Product created:', result);
        setSubmitStatus('success');
        setFormData({ name: '', price: 0, sku: '', description: '' });
      } else {
        setSubmitStatus('error');
      }
    } catch (error) {
      console.error('Network error:', error);
      setSubmitStatus('error');
    }
  };

  const handleChange = (field: keyof Product, value: string | number) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
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
      <h2>Create Product</h2>
      <p className="form-subtitle">Validation powered by generated fluentvalidation-ts</p>

      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="name">Product Name:</label>
          <input
            id="name"
            type="text"
            value={formData.name || ''}
            onChange={(e) => handleChange('name', e.target.value)}
            className={errors.name ? 'error' : ''}
            placeholder="e.g., Premium Widget"
          />
          {errors.name && <span className="error-message">{errors.name}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="sku">SKU:</label>
          <input
            id="sku"
            type="text"
            value={formData.sku || ''}
            onChange={(e) => handleChange('sku', e.target.value.toUpperCase())}
            className={errors.sku ? 'error' : ''}
            placeholder="Format: XXX-9999"
          />
          {errors.sku && <span className="error-message">{errors.sku}</span>}
          <small style={{ color: '#666', fontSize: '0.8rem' }}>
            Must follow format: XXX-9999 (e.g., ABC-1234)
          </small>
        </div>

        <div className="form-group">
          <label htmlFor="price">Price:</label>
          <input
            id="price"
            type="number"
            step="0.01"
            value={formData.price || ''}
            onChange={(e) => handleChange('price', parseFloat(e.target.value) || 0)}
            className={errors.price ? 'error' : ''}
          />
          {errors.price && <span className="error-message">{errors.price}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="description">Description:</label>
          <textarea
            id="description"
            value={formData.description || ''}
            onChange={(e) => handleChange('description', e.target.value)}
            className={errors.description ? 'error' : ''}
            rows={4}
            style={{ resize: 'vertical', fontFamily: 'inherit' }}
          />
          {errors.description && <span className="error-message">{errors.description}</span>}
        </div>

        <button type="submit" className="submit-btn">
          Create Product
        </button>

        {submitStatus === 'success' && (
          <div className="success-message">✓ Product created successfully!</div>
        )}
        {submitStatus === 'error' && (
          <div className="error-message">✗ Please fix validation errors</div>
        )}
      </form>
    </div>
  );
}
