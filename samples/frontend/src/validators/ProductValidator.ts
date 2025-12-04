import { Validator } from 'fluentvalidation-ts';

export type Product = {
  id: number;
  name: string;
  price: number;
  sku: string;
  description: string;
};

export class ProductValidator extends Validator<Product> {
  constructor() {
    super();

    this.ruleFor('id')
      .notNull()
      .withMessage('Product ID is required');

    this.ruleFor('name')
      .notEmpty()
      .withMessage('Product name is required')
      .maxLength(200)
      .withMessage('Product name cannot exceed 200 characters');

    this.ruleFor('price')
      .greaterThan(0)
      .withMessage('Price must be greater than 0')
      .lessThanOrEqualTo(100000)
      .withMessage('Price cannot exceed 100,000');

    this.ruleFor('sku')
      .notEmpty()
      .withMessage('SKU is required')
      .matches(new RegExp('^[A-Z]{3}-\\d{4}$'))
      .withMessage('SKU must follow format: XXX-9999');

    this.ruleFor('description')
      .maxLength(1000)
      .withMessage('Description cannot exceed 1000 characters');
  }
}