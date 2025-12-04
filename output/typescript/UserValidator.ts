import { Validator } from 'fluentvalidation-ts';

export type User = {
  id: number;
  email: string;
  age: number;
  name: string;
};

export class UserValidator extends Validator<User> {
  constructor() {
    super();

    this.ruleFor('id')
      .notNull()
      .withMessage('User ID is required');

    this.ruleFor('email')
      .notEmpty()
      .withMessage('Email is required')
      .emailAddress()
      .withMessage('Please provide a valid email address')
      .maxLength(255)
      .withMessage('Email must not exceed 255 characters');

    this.ruleFor('age')
      .inclusiveBetween(18, 120)
      .withMessage('Age must be between 18 and 120');

    this.ruleFor('name')
      .notEmpty()
      .withMessage('Name is required')
      .length(2, 100)
      .withMessage('Name must be between 2 and 100 characters');
  }
}