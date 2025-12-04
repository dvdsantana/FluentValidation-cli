import { Validator } from 'fluentvalidation-ts';

export type Address = {
  street: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
};

export class AddressValidator extends Validator<Address> {
  constructor() {
    super();

    this.ruleFor('street')
      .notEmpty()
      .withMessage('Street address is required')
      .length(5, 200)
      .withMessage('Street address must be between 5 and 200 characters');

    this.ruleFor('city')
      .notEmpty()
      .withMessage('City is required')
      .maxLength(100)
      .withMessage('City cannot exceed 100 characters');

    this.ruleFor('state')
      .notEmpty()
      .withMessage('State is required')
      .length(2, 2)
      .withMessage('State must be a 2-letter code');

    this.ruleFor('zipCode')
      .notEmpty()
      .withMessage('ZIP code is required')
      .matches(new RegExp('^\\d{5}(-\\d{4})?$'))
      .withMessage('ZIP code must be in format 12345 or 12345-6789');

    this.ruleFor('country')
      .notEmpty()
      .withMessage('Country is required')
      .length(2, 2)
      .withMessage('Country must be a 2-letter ISO code');
  }
}