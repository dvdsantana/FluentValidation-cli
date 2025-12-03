import { Validator } from 'fluentvalidation-ts';

export interface User {
  username: string;
  email: string;
  age: number;
}

export class UserValidator extends Validator<User> {
  constructor() {
    super();

    this.ruleFor('username').notEmpty().withMessage('Username is required').length(3, 20).withMessage('Username must be between 3 and 20 chars');
    this.ruleFor('email').notEmpty().emailAddress();
    this.ruleFor('age').greaterThan(18).lessThan(100);
  }
}
