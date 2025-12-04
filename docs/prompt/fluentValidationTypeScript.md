## ambientContext.md

---
id: ambientContext
title: Ambient Context
---

Sometimes your validation logic will need to depend on external, or "ambient", context that isn't part of your form model. With **fluentvalidation-ts** validators are just classes, so you can make use of constructor arguments to inject dependencies.

## The Gist

You can inject external dependencies into your validators using constructor arguments:

```typescript
type FormModel = {
  age: number;
};

class FormValidator extends Validator<FormModel> {
  // highlight-next-line
  constructor(country: string) {
    super();

    this.ruleFor('age')
      // highlight-next-line
      .greaterThanOrEqualTo(country === 'US' ? 21 : 18);
  }
}
```

This approach means that you need to instantiate a new instance of your validator every time the ambient context changes, so there is potentially a performance cost involved.

Usage of the example validator from above might look something like this:

```typescript
const ukValidator = new FormValidator('UK');
const usValidator = new FormValidator('US');

const pubGoer = { age: 20 };

const ukResult = ukValidator.validate(pubGoer); // {}
const usResult = usValidator.validate(pubGoer); // { age: 'Value must be greater than or equal to 21' }
```


## arrayProperties.md

---
id: arrayProperties
title: Array Properties
---

Validating array properties is made easy with the [`.ruleForEach`](api/core/ruleForEach.md) method.

The `.ruleForEach` method works almost exactly the same as the [`.ruleFor`](api/core/ruleFor.md) method, so it's worth reading up on that first if you haven't already.

## The Gist

You can validate an array property using the `.ruleFor` method:

```typescript
this.ruleFor('scores').must(
  (scores) => scores.filter((score) => score < 0 || score > 100).length === 0
);
```

Alternatively, you can use the `.ruleForEach` method:

```typescript
this.ruleForEach('scores').greaterThanOrEqualTo(0).lessThanOrEqualTo(100);
```


## asyncValidator.md

---
id: asyncValidator
title: AsyncValidator
---

The `AsyncValidator<TModel>` generic class is an extension of [`Validator<TModel>`](api/core/validator.md) that has additional async rules available (most notably [`.mustAsync`](api/rules/mustAsync.md) and [`.setAsyncValidator`](api/rules/setAsyncValidator.md)).

```typescript
import { AsyncValidator } from 'fluentvalidation-ts';
```

Defining an async validator for a model of type `TModel` works exactly the same as defining a standard validator - all you have to do is define a class which extends `AsyncValidator<TModel>` (as opposed to `Validator<TModel>`) and specify some rules in the constructor using the [`.ruleFor`](api/core/ruleFor.md) and [`.ruleForEach`](api/core/ruleForEach.md) methods.

```typescript
type FormModel = { username: string };

class FormValidator extends AsyncValidator<FormModel> {
  constructor() {
    super();

    this.ruleFor('username').mustAsync(async (username) =>
       await api.usernameIsAvailable(username);
    )
    .withMessage('This username is already taken');
  }
}
```

To actually validate an instance of your model, simply create an instance of your validator and pass your model to the `.validateAsync` method. As the name suggests this method is **asynchronous**, so be sure to `await` the result or use Promise callback methods (i.e. `.then` and `.catch`).

Note that the synchronous `.validate` method is **not available** on an instance of `AsyncValidator`, you must always use the `.validateAsync` method.

```typescript
const formValidator = new FormValidator();

const validResult = await formValidator.validateAsync({
  username: 'ajp_dev123',
});
// ✔ {}

const invalidResult = await formValidator.validateAsync({
  username: 'ajp_dev',
});
// ❌ { username: 'This username is already taken' }
```

A call to `.validateAsync` returns a `Promise` that resolves to an object of type [`ValidationErrors<TModel>`](api/core/validationErrors.md), which describes the validity of the given value.


## customRules.md

---
id: customRules
title: Custom Rules
---

One of the main features of **fluentvalidation-ts** is that it is fully extensible, allowing you define your own custom validation logic and inject it via the [`.must`](api/rules/must.md) rule.

The [documentation page](api/rules/must.md) for the `.must` rule contains several [examples](api/rules/must.md#examples) that demonstrate the different ways in which you can define and consume custom rules, as well as a full [API reference](api/rules/must.md#reference) which outlines everything in detail.

## The Gist

Custom validation logic is defined by way of a **predicate** function, which takes a value and returns a boolean (true/false) value indicating whether or not the value is valid.

You can pass custom validation logic directly into the `.must` rule with a predicate:

```typescript
this.ruleFor('numberOfSocks').must((numberOfSocks) => numberOfSocks % 2 === 0);
```

If you want to reuse the logic, you could pull it out into a named function:

```typescript
const beEven = (value: number) => value % 2 === 0;
```

Then you can just pass the named function into `.must`, like so:

```typescript
this.ruleFor('numberOfSocks').must(beEven);
```

The predicate function can also depend on the value of the model as well as the value of the property:

```typescript
this.ruleFor('numberOfSocks').must(
  // highlight-next-line
  (numberOfSocks, model) => numberOfSocks === 2 * model.numberOfPants
);
```

You can define groups of rules by forming arrays:

```typescript
const beEven = (value: number) => value % 2 === 0;
const bePositive = (value: number) => value > 0;

// highlight-next-line
const beEvenAndPositive = [beEven, bePositive];
```

These arrays can be passed directly to the `.must` rule:

```typescript
this.ruleFor('numberOfSocks').must(beEvenAndPositive);
```

You can also attach a custom message to your rule, alongside the predicate:

```typescript
const beEven = {
  predicate: (value: number) => value % 2 === 0,
  // highlight-next-line
  message: 'Please enter an even number',
};
```

As before, you just pass this into the `.must` rule directly:

```typescript
this.ruleFor('numberOfSocks').must(beEven);
```

Again, you can use arrays to compose rules together:

```typescript
const beEven = {
  predicate: (value: number) => value % 2 === 0,
  message: 'Please enter an even number',
};

const bePositive = {
  predicate: (value: number) => value > 0,
  message: 'Please enter a positive number',
};

// highlight-next-line
const beEvenAndPositive = [beEven, bePositive];
```

You can even compose groups of rules together by spreading or concatenating the arrays:

```typescript
const newRuleGroup = [...ruleGroup, ...otherRuleGroup];
```


## emailAddress.md

---
id: emailAddress
title: '.emailAddress'
---

The `.emailAddress` rule is used to ensure that the value of a given `string` property is a valid email address.

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  contactEmail: string;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('contactEmail').emailAddress();
  }
}

const formValidator = new FormValidator();

formValidator.validate({ contactEmail: 'foo@example.com' });
// ✔ {}

formValidator.validate({ contactEmail: 'foo' });
// ❌ { contactEmail: 'Not a valid email address' }
```

## Reference

### `.emailAddress()`

A string validation rule which ensures that the given property is a valid email address.

## Example Message

> Not a valid email address


## equal.md

---
id: equal
title: '.equal'
---

The `.equal` rule is used to ensure that the value of a given property is equal to a given value.

Note that this rule uses **strict** equality (i.e. the `===` operator) and may not work as intended for object or array values.

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  acceptsTermsAndConditions: boolean;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('acceptsTermsAndConditions').equal(true);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ acceptsTermsAndConditions: true });
// ✔ {}

formValidator.validate({ acceptsTermsAndConditions: false });
// ❌ { acceptsTermsAndConditions: `Must equal 'true'` }
```

## Reference

### `.equal(comparisonValue: TValue)`

A base validation rule which takes in a value and ensures that the given property is equal to that value.

### `TValue`

Matches the type of the property that the rule is applied to.

## Example Message

> Must equal '`[comparisonValue]`'


## exclusiveBetween.md

---
id: exclusiveBetween
title: '.exclusiveBetween'
---

The `.exclusiveBetween` rule is used to ensure that the value of a given `number` property is exclusively between the given bounds (i.e. greater than the lower bound and less than the upper bound).

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  score: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('score').exclusiveBetween(0, 10);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ score: 5 });
// ✔ {}

formValidator.validate({ score: 0 });
// ❌ { score: 'Value must be between 0 and 10 (exclusive)' }
```

## Reference

### `.exclusiveBetween(lowerBound: number, upperBound: number)`

A number validation rule which takes in a lower bound and upper bound and ensures that the given property is exclusively between them (i.e. greater than the lower bound and less than the upper bound).

## Example Message

> Value must be between `[lowerBound]` and `[upperBound]` (exclusive)


## formik.md

---
id: formik
title: Formik
---

When I first wrote **fluentvalidation-ts**, I had seamless integration with [Formik](https://formik.org/) in mind.

The [`ValidationErrors`](/docs/api/core/ValidationErrors) object returned by the [`.validate`](/docs/api/core/validator#validate) function has been designed to "just work" with Formik, so you can start using the two together with minimal effort.

If you're not familiar with Formik, it's a fantastic library for writing forms in [React](https://react.dev/).

## Usage

To use **fluentvalidation-ts** with Formik, simply define a `Validator` for your form model, instantiate an instance of your validator, then pass the validator's [`.validate`](https://formik.org/docs/guides/validation#validate) method to Formik's `validate` prop:

```tsx
import { Formik } from 'formik';
import { Validator } from 'fluentvalidation-ts';

type FormModel = { username: string };

// highlight-start
class MyFormValidator extends Validator<FormModel> {
  constructor() {
    super();
    this.ruleFor('username').notEmpty().withMessage('Please enter your username');
  }
}

const formValidator = new MyFormValidator();
// highlight-end

export const MyForm = () => (
  <Formik<FormModel>
    // highlight-next-line
    validate={formValidator.validate}
    ...
  >
    ...
  </Formik>
);
```


## greaterThan.md

---
id: greaterThan
title: '.greaterThan'
---

The `.greaterThan` rule is used to ensure that the value of a given `number` property is strictly greater than a given value.

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  quantity: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('quantity').greaterThan(0);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ quantity: 2 });
// ✔ {}

formValidator.validate({ quantity: 0 });
// ❌ { quantity: 'Value must be greater than 0' }
```

## Reference

### `.greaterThan(threshold: number)`

A number validation rule which takes in a threshold and ensures that the given property is strictly greater than it.

## Example Message

> Value must be greater than `[threshold]`


## greaterThanOrEqualTo.md

---
id: greaterThanOrEqualTo
title: '.greaterThanOrEqualTo'
---

The `.greaterThanOrEqualTo` rule is used to ensure that the value of a given `number` property is greater than or equal to a given value.

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  age: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('age').greaterThanOrEqualTo(18);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ age: 18 });
// ✔ {}

formValidator.validate({ age: 16 });
// ❌ { age: 'Value must be greater than or equal to 18' }
```

## Reference

### `.greaterThanOrEqualTo(threshold: number)`

A number validation rule which takes in a threshold and ensures that the given property is greater than or equal to it.

## Example Message

> Value must be greater than or equal to `[threshold]`


## inclusiveBetween.md

---
id: inclusiveBetween
title: '.inclusiveBetween'
---

The `.inclusiveBetween` rule is used to ensure that the value of a given `number` property is inclusively between the given bounds (i.e. greater than or equal to the lower bound and less than or equal to the upper bound).

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  percentageComplete: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('percentageComplete').inclusiveBetween(0, 100);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ percentageComplete: 50 });
// ✔ {}

formValidator.validate({ percentageComplete: 110 });
// ❌ { percentageComplete: 'Value must be between 0 and 100 (inclusive)' }
```

## Reference

### `.inclusiveBetween(lowerBound: number, upperBound: number)`

A number validation rule which takes in a lower bound and upper bound and ensures that the given property is inclusively between them (i.e. greater than or equal to the lower bound and less than or equal to the upper bound).

## Example Message

> Value must be between `[lowerBound]` and `[upperBound]` (inclusive)


## length.md

---
id: length
title: '.length'
---

The `.length` rule is used to ensure that the length of a given `string` property is inclusively between the given bounds (i.e. greater than or equal to the lower bound and less than or equal to the upper bound).

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  voucherCode: string;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('voucherCode').length(5, 10);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ voucherCode: 'ABC44' });
// ✔ {}

formValidator.validate({ voucherCode: 'ZZ' });
// ❌ { voucherCode: 'Value must be between 5 and 10 characters long' }
```

## Reference

### `.length(lowerBound: number, upperBound: number)`

A string validation rule which takes in a lower bound and upper bound and ensures that the length of the given property is inclusively between them (i.e. greater than or equal to the lower bound and less than or equal to the upper bound).

## Example Message

> Value must be between `[lowerBound]` and `[upperBound]` characters long


## lessThan.md

---
id: lessThan
title: '.lessThan'
---

The `.lessThan` rule is used to ensure that the value of a given `number` property is strictly less than a given value.

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  bagWeightInKilograms: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('bagWeightInKilograms').lessThan(20);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ bagWeightInKilograms: 18.5 });
// ✔ {}

formValidator.validate({ bagWeightInKilograms: 22.8 });
// ❌ { bagWeightInKilograms: 'Value must be less than 20' }
```

## Reference

### `.lessThan(threshold: number)`

A number validation rule which takes in a threshold and ensures that the given property is strictly less than it.

## Example Message

> Value must be less than `[threshold]`


## lessThanOrEqualTo.md

---
id: lessThanOrEqualTo
title: '.lessThanOrEqualTo'
---

The `.lessThanOrEqualTo` rule is used to ensure that the value of a given `number` property is less than or equal to a given value.

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  passengers: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('passengers').lessThanOrEqualTo(4);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ passengers: 4 });
// ✔ {}

formValidator.validate({ passengers: 6 });
// ❌ { passengers: 'Value must be less than or equal to 4' }
```

## Reference

### `.lessThanOrEqualTo(threshold: number)`

A number validation rule which takes in a threshold and ensures that the given property is less than or equal to it.

## Example Message

> Value must be less than or equal to `[threshold]`


## matches.md

---
id: matches
title: '.matches'
---

The `.matches` rule is used to ensure that the value of a given `string` property matches the given [regular expression](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/RegExp).

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  price: string;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('price').matches(new RegExp('^([0-9])+.([0-9]){2}$'));
  }
}

const formValidator = new FormValidator();

formValidator.validate({ price: '249.99' });
// ✔ {}

formValidator.validate({ price: '15' });
// ❌ { price: 'Value does not match the required pattern' }
```

## Reference

### `.matches(pattern: RegExp)`

A string validation rule which takes in a [regular expression](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/RegExp) and ensures that the given property matches it.

## Example Message

> Value does not match the required pattern


## maxLength.md

---
id: maxLength
title: '.maxLength'
---

The `.maxLength` rule is used to ensure that the length of a given `string` property is less than or equal to a given value.

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  username: string;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('username').maxLength(20);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ username: 'AlexPotter' });
// ✔ {}

formValidator.validate({ username: 'ThisUsernameIsFarTooLong' });
// ❌ { username: 'Value must be no more than 20 characters long' }
```

## Reference

### `.maxLength(upperBound: number)`

A string validation rule which takes in an upper bound and ensures that the length of the given property is less than or equal to it.

## Example Message

> Value must be no more than `[upperBound]` characters long


## minLength.md

---
id: minLength
title: '.minLength'
---

The `.minLength` rule is used to ensure that the length of a given `string` property is greater than or equal to a given value.

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  password: string;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('password').minLength(6);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ password: 'supersecret' });
// ✔ {}

formValidator.validate({ password: 'foo' });
// ❌ { password: 'Value must be at least 6 characters long' }
```

## Reference

### `.minLength(lowerBound: number)`

A string validation rule which takes in a lower bound and ensures that the length of the given property is greater than or equal to it.

## Example Message

> Value must be at least `[lowerBound]` characters long


## must.md

---
id: must
title: '.must'
---

The `.must` rule is used to ensure that a particular property is valid according to a given predicate (or array of predicates).

You can either specify a predicate on its own, or a predicate along with the message to use when the validation fails. You can even pass an array of predicates, which allows you to compose custom validation rules together.

This rule is very useful, as it allows you to define reusable validation logic which can be shared across validators for many different models.

## Examples

### Predicate dependent on value

In this example we specify a predicate on its own, which is dependent only on the value of the property we're validating.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  attendees: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    // highlight-next-line
    this.ruleFor('attendees').must((attendees) => attendees % 2 === 0);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ attendees: 46 });
// ✔ {}

formValidator.validate({ attendees: 13 });
// ❌ { attendees: 'Value is not valid' }
```

### Predicate dependent on value and model

In this example we specify a predicate on its own, which is dependent on both the value of the property we're validating and the model as a whole.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  yearsInCurrentJob: number;
  age: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    // highlight-start
    this.ruleFor('yearsInCurrentJob').must(
      (yearsInCurrentJob, formModel) => yearsInCurrentJob < formModel.age
    );
    // highlight-end
  }
}

const formValidator = new FormValidator();

formValidator.validate({ yearsInCurrentJob: 3, age: 30 });
// ✔ {}

formValidator.validate({ yearsInCurrentJob: 99, age: 30 });
// ❌ { yearsInCurrentJob: 'Value is not valid' }
```

### Predicate and message

In this example we define a named variable which wraps up both a predicate and message, so that we can easily reuse the validation logic.

```typescript
import { Validator } from 'fluentvalidation-ts';

// highlight-start
const bePositive = {
  predicate: (value: number) => value > 0,
  message: 'Value must be positive',
};
// highlight-end

type FormModel = {
  age: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    // highlight-next-line
    this.ruleFor('age').must(bePositive);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ age: 30 });
// ✔ {}

formValidator.validate({ age: -1 });
// ❌ { age: 'Value must be positive' }
```

### Predicate and dynamic message

In this example we again define a named variable which wraps up both a predicate and message, but this time the message is generated dynamically based on the value of the property being validated and the model as a whole.

```typescript
import { Validator } from 'fluentvalidation-ts';

// highlight-start
const matchTheUsername = {
  predicate: (value: string, model: FormModel) => value === model.username,
  message: (value: string, model: FormModel) =>
    `Value (${value}) does not match the username (${model.username})`,
};
// highlight-end

type FormModel = {
  username: string;
  retypeUsername: string;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    // highlight-next-line
    this.ruleFor('retypeUsername').must(matchTheUsername);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ username: 'Alex', retypeUsername: 'Alex' });
// ✔ {}

formValidator.validate({ username: 'foo', retypeUsername: 'bar' });
// ❌ { retypeUsername: 'Value (bar) does not match the username (foo)' }
```

### Array of predicates

In this example we define two named variables, which both wrap up a predicate and message. We then define a third named variable, which is an array formed from the previous two.

This usage of the `.must` rule allows us to compose validation rules together - each element is applied in turn and the first failing one produces the error that is returned in the validation errors object.

Note that we composed two rule definitions in this example, but the array can be a mix of both predicates and rule definitions.

```typescript
import { Validator } from 'fluentvalidation-ts';

// highlight-start
const beEven = {
  predicate: (value: number) => value % 2 === 0,
  message: 'Please enter an even number',
};

const bePositive = {
  predicate: (value: number) => value > 0,
  message: 'Please enter a positive number',
};

const beEvenAndPositive = [beEven, bePositive];
// highlight-end

type FormModel = {
  numberOfSocks: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    // highlight-next-line
    this.ruleFor('numberOfSocks').must(beEvenAndPositive);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ numberOfSocks: 8 });
// ✔ {}

formValidator.validate({ numberOfSocks: -2 });
// ❌ { numberOfSocks: 'Please enter a positive number' }
```

## Reference

The `.must` rule is the most complex of all the built-in rules. You may wish to refer to the examples above to help you understand the different variations of this rule.

### `.must(predicate: SimplePredicate<TModel, TValue>)`

A validation rule which takes in a simple predicate function and ensures that the given property is valid according to that predicate function.

### `.must(predicateAndMessage: SimplePredicateWithMessage<TModel, TValue>)`

A validation rule which takes in a predicate argument that specifies both a simple predicate function and a message (or message generator), and ensures that the given property is valid according to the given predicate function (exposing the relevant message if validation fails).

### `.must(predicates: Array<SimplePredicate<TModel, TValue> | SimplePredicateWithMessage<TModel, TValue>>)`

A validation rule which takes in an array of simple predicate functions and/or predicate function and message (or message generator) pairs, and ensures that the given property is valid according to each one (exposing a relevant message for the first failing predicate if validation fails).

### `SimplePredicateWithMessage<TModel, TValue>`

Equivalent to `{ predicate: SimplePredicate<TModel, TValue>; message: string | MessageGenerator<TModel, TValue> }`

An object that specifies both a simple predicate function and a message (or message generator). The predicate function is used to determine whether a given value is valid, and the message (either explicit or generated) is used in the validation errors object if validation fails.

### `SimplePredicate<TModel, TValue>`

Equivalent to `(value: TValue, model: TModel) => boolean`.

A simple predicate is a function which accepts the value of the property being validated and the value of the model as a whole, and returns a `boolean` indicating whether the property is valid or not.

A return value of `true` indicates that the property is valid ✔.

Conversely, a return value of `false` indicates that the property is invalid ❌.

### `MessageGenerator<TModel, TValue>`

Equivalent to `(value: TValue, model: TModel) => string`.

A function which accepts both the value being validated and the model as a whole, and returns an appropriate error message.

### `TValue`

Matches the type of the property that the rule is applied to.

### `TModel`

Matches the type of the base model.

## Example Message

> Value is not valid


## mustAsync.md

---
id: mustAsync
title: '.mustAsync'
---

The `.mustAsync` rule is one of the special async rules that become available when you extend from [`AsyncValidator`](api/core/asyncValidator.md) as opposed to just [`Validator`](api/core/validator.md).

This rule works exactly the same as the [`.must`](api/rules/must.md) rule, except that it takes an async predicate function. This allows you to do things like define custom validation rules which perform API requests (e.g. checking if a username is already taken).

All the various overloads for the [`.must`](api/rules/must.md) rule are also available for the `.mustAsync` rule - the only difference is that your predicate function must be async (i.e. have a return type of `Promise<boolean>` instead of `boolean`).

## Examples

The documentation page for the [`.must`](api/rules/must.md) rule includes a full list of examples demonstrating the different overloads that are available.

These are all relevant to the `.mustAsync` rule too, just replace `Validator` with `AsyncValidator`, `.must` with `.mustAsync`, and synchronous predicate functions with asynchronous ones.

### Predicate dependent on value

In this example we specify an async predicate on its own, which is dependent only on the value of the property we're validating.

```typescript
import { AsyncValidator } from 'fluentvalidation-ts';

type FormModel = {
  username: string;
};

class FormValidator extends AsyncValidator<FormModel> {
  constructor() {
    super();

    // highlight-start
    this.ruleFor('username').mustAsync(
      async (username) => await api.usernameIsAvailable(username)
    );
    // highlight-end
  }
}

const formValidator = new FormValidator();

await formValidator.validateAsync({ username: 'ajp_dev123' });
// ✔ {}

await formValidator.validateAsync({ username: 'ajp_dev' });
// ❌ { username: 'Value is not valid' }
```

## Reference

The `.mustAsync` rule is one of the more complex built-in rules. You may wish to refer to the examples on the documentation page for the [`.must`](api/rules/must.md) rule to help you understand the different variations of this rule.

### `.mustAsync(predicate: SimpleAsyncPredicate<TModel, TValue>)`

A validation rule which takes in a simple async predicate function and ensures that the given property is valid according to that predicate function.

### `.mustAsync(predicateAndMessage: SimpleAsyncPredicateWithMessage<TModel, TValue>)`

A validation rule which takes in a definition that specifies both an async predicate function and a message (or message generator), and ensures that the given property is valid according to the given predicate function (exposing the relevant message if validation fails).

### `.mustAsync(definitions: Array<SimpleAsyncPredicate<TModel, TValue> | SimpleAsyncPredicateWithMessage<TModel, TValue>>)`

A validation rule which takes in an array of async predicate functions and/or predicate function and message (or message generator) pairs, and ensures that the given property is valid according to each one (exposing a relevant message for the first failing predicate if validation fails).

### `SimpleAsyncPredicateWithMessage<TModel, TValue>`

Equivalent to `{ predicate: SimpleAsyncPredicate<TModel, TValue>; message: string | MessageGenerator<TModel, TValue> }`

An object that specifies both an async predicate function and a message (or message generator). The predicate function is used to determine whether a given value is valid, and the message (either explicit or generated) is used in the validation errors object if validation fails.

### `SimpleAsyncPredicate<TModel, TValue>`

Equivalent to `(value: TValue, model: TModel) => Promise<boolean>`.

A simple predicate is an async function which accepts the value of the property being validated and the value of the model as a whole, and returns a `Promise<boolean>` indicating whether the property is valid or not.

A return value that resolves to `true` indicates that the property is valid ✔.

Conversely, a return value that resolves to `false` indicates that the property is invalid ❌.

### `MessageGenerator<TModel, TValue>`

Equivalent to `(value: TValue, model: TModel) => string`.

A function which accepts both the value being validated and the model as a whole, and returns an appropriate error message.

### `TValue`

Matches the type of the property that the rule is applied to.

### `TModel`

Matches the type of the base model.

## Example Message

> Value is not valid


## notEmpty.md

---
id: notEmpty
title: '.notEmpty'
---

The `.notEmpty` rule is used to ensure that the value of a given `string` property is not the empty string, or formed entirely of whitespace.

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  name: string;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('name').notEmpty();
  }
}

const formValidator = new FormValidator();

formValidator.validate({ name: 'Alex' });
// ✔ {}

formValidator.validate({ name: '   ' });
// ❌ { name: 'Value cannot be empty' }
```

## Reference

### `.notEmpty()`

A string validation rule which ensures that the given property is not the empty string, or formed entirely of whitespace.

## Example Message

> Value cannot be empty


## notEqual.md

---
id: notEqual
title: '.notEqual'
---

The `.notEqual` rule is used to ensure that the value of a given property is not equal to a given value.

Note that this rule uses **strict** inequality (i.e. the `!==` operator) and may not work as intended for object or array values.

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  acceptsTermsAndConditions: boolean;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('acceptsTermsAndConditions').notEqual(false);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ acceptsTermsAndConditions: true });
// ✔ {}

formValidator.validate({ acceptsTermsAndConditions: false });
// ❌ { acceptsTermsAndConditions: `Value must not equal 'false'` }
```

## Reference

### `.notEqual(comparisonValue: TValue)`

A base validation rule which takes in a value and ensures that the given property is not equal to that value.

### `TValue`

Matches the type of the property that the rule is applied to.

## Example Message

> Value must not equal '`[comparisonValue]`'


## notNull.md

---
id: notNull
title: '.notNull'
---

The `.notNull` rule is used to ensure that the value of a given property is not `null` (including `undefined` by default, though this is configurable).

:::tip

If you only want to check for `undefined` values, you may use the [`.notUndefined`](./notUndefined.md) rule instead.

:::

## Examples

### Default Usage

If you don't specify any options, the rule will check that the given value is not `null` or `undefined`.

In other words, the `includeUndefined` option is defaulted to `true` - this decision was made to avoid introducing a breaking change.

In this setup, both `null` and `undefined` values will be considered invalid.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  customerId?: number | null;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('customerId').notNull();
  }
}

const formValidator = new FormValidator();

formValidator.validate({ customerId: 100 });
// ✔ {}

formValidator.validate({ customerId: null });
// ❌ { customerId: 'Value cannot be null' }

formValidator.validate({ customerId: undefined });
// ❌ { customerId: 'Value cannot be null' }

formValidator.validate({});
// ❌ { customerId: 'Value cannot be null' }
```

### Excluding `undefined`

The behaviour of the `.notNull` rule can be made "strict" (in the sense that it only checks for `null` and not `undefined`) by passing the `includeUndefined` option as `false`.

In this setup, `undefined` values will be allowed, and only `null` values will be considered invalid.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  customerId?: number | null;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    // highlight-next-line
    this.ruleFor('customerId').notNull({ includeUndefined: false });
  }
}

const formValidator = new FormValidator();

formValidator.validate({ customerId: 100 });
// ✔ {}

formValidator.validate({ customerId: null });
// ❌ { customerId: 'Value cannot be null' }

// highlight-start
formValidator.validate({ customerId: undefined });
// ✔ {}

formValidator.validate({});
// ✔ {}
// highlight-end
```

## Reference

### `.notNull(ruleOptions?: NotNullRuleOptions)`

A validation rule which ensures that the given property is not `null` (or `undefined`, depending on the value of `ruleOptions`).

The default value of `ruleOptions` is `{ includeUndefined: true }`, meaning that both `null` and `undefined` values will be considered invalid.

### `NotNullRuleOptions`

Equivalent to `{ includeUndefined: boolean }`, where the `includeUndefined` property determines whether `undefined` values should be considered invalid.

When `includeUndefined` is `true`, both `null` and `undefined` values will be considered invalid.

When `includeUndefined` is `false`, only `null` values will be considered invalid, and `undefined` values will be allowed.

## Example Message

> Value cannot be null


## notUndefined.md

---
id: notUndefined
title: '.notUndefined'
---

The `.notUndefined` rule is used to ensure that the value of a given property is not `undefined`.

:::note

Note that this rule considers `null` values to be **valid**. If you need to disallow both `null` and `undefined` values (or just `null` values), you may use the [`.notNull`](./notNull.md) rule instead.

:::

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  customerId?: number | null;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('customerId').notUndefined();
  }
}

const formValidator = new FormValidator();

formValidator.validate({ customerId: 100 });
// ✔ {}

formValidator.validate({ customerId: null });
// ✔ {}

formValidator.validate({});
// ❌ { customerId: 'Value cannot be undefined' }

formValidator.validate({ customerId: undefined });
// ❌ { customerId: 'Value cannot be undefined' }
```

## Reference

### `.notUndefined()`

A validation rule which ensures that the given property is not `undefined`.

## Example Message

> Value cannot be undefined


## null.md

---
id: nullRule
title: '.null'
---

The `.null` rule is used to ensure that the value of a given property is `null` (or `undefined` by default, though this is configurable).

:::tip

If you only want to check for `undefined` values, you may use the [`.undefined`](./undefined.md) rule instead.

:::

## Examples

### Default Usage

If you don't specify any options, the rule will check that the given value is `null` or `undefined`.

In other words, the `includeUndefined` option is defaulted to `true` - this decision was made to avoid introducing a breaking change.

In this setup, both `null` and `undefined` values will be considered valid.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  apiError?: string | null;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('apiError').null();
  }
}

const formValidator = new FormValidator();

formValidator.validate({ apiError: null });
// ✔ {}

formValidator.validate({ apiError: 'Failed to fetch data from the API' });
// ❌ { apiError: 'Value must be null' }

formValidator.validate({ apiError: undefined });
// ✔ {}

formValidator.validate({});
// ✔ {}
```

### Excluding `undefined`

The behaviour of the `.null` rule can be made "strict" (in the sense that it only checks for `null` and not `undefined`) by passing the `includeUndefined` option as `false`.

In this setup, `undefined` values will be considered invalid, and only `null` values will be allowed.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  apiError?: string | null;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('apiError').null({ includeUndefined: false });
  }
}

const formValidator = new FormValidator();

formValidator.validate({ apiError: null });
// ✔ {}

formValidator.validate({ apiError: 'Failed to fetch data from the API' });
// ❌ { apiError: 'Value must be null' }

// highlight-start
formValidator.validate({ apiError: undefined });
// ❌ { apiError: 'Value must be null' }

formValidator.validate({});
// ❌ { apiError: 'Value must be null' }
// highlight-end
```

## Reference

### `.null(ruleOptions?: NullRuleOptions)`

A validation rule which ensures that the given property is `null` (or `undefined`, depending on the value of `ruleOptions`).

The default value of `ruleOptions` is `{ includeUndefined: true }`, meaning that both `null` and `undefined` values will be considered valid.

### `NullRuleOptions`

Equivalent to `{ includeUndefined: boolean }`, where the `includeUndefined` property determines whether `undefined` values should be considered valid.

When `includeUndefined` is `true`, both `null` and `undefined` values will be considered valid.

When `includeUndefined` is `false`, only `null` values will be considered valid, and `undefined` values will be considered invalid.

## Example Message

> Value must be null


## objectProperties.md

---
id: objectProperties
title: Object Properties
---

Object properties can be validated by way of the [`.setValidator`](api/rules/setValidator.md) rule.

The [documentation page](api/rules/setValidator.md) for the `.setValidator` rule contains several [examples](api/rules/setValidator.md#examples) that demonstrate the different ways in which you can use it, as well as a full [API reference](api/rules/setValidator.md#reference) which outlines everything in detail.

## The Gist

You can validate an object property using the built-in rules:

```typescript
this.ruleFor('pet')
  .notNull()
  .must((pet) => pet.age >= 0)
  .must((pet) => pet.name !== '');
```

Alternatively, you can define a validator for the type of the object property:

```typescript
class PetValidator extends Validator<Pet> {
  constructor() {
    super();
    this.ruleFor('age').greaterThanOrEqualTo(0);
    this.ruleFor('name').notEmpty();
  }
}

const petValidator = new PetValidator();
```

This can then be passed in with the `.setValidator` rule:

```typescript
this.ruleFor('pet')
  .notNull()
  // highlight-next-line
  .setValidator(() => petValidator);
```


## overview.md

---
id: overview
title: Overview
---

Front-end validation is a must-have for any project that involves forms, but the requirements vary hugely. You might have a simple sign-up form with a few text fields, or a complex configuration page with collections and deeply nested fields.

There are plenty of libraries out there which help you to solve the problem of front-end validation, but all the ones I've tried have felt lacking in one aspect or another - whether that's TypeScript support, their capacity to handle complex requirements, the ability to define your own reusable validation logic, or just the expressiveness of the API.

So I wrote **fluentvalidation-ts**, a tiny library that is:

- Designed for TypeScript
- Simple yet powerful
- Fully extensible

Whatever your validation needs, **fluentvalidation-ts** can handle them.

## Compatibility

**fluentvalidation-ts** is completely framework-agnostic, so you can use it with any front-end framework or library. It has no dependencies, and is designed to be as lightweight as possible. Having said that, it has primarily been designed to integrate seamlessly with popular form libraries for React - see the guides on [Formik](/docs/guides/formik) and [React Hook Form](/docs/guides/reactHookForm) for more information.

## Influences

If you've ever worked on a .NET API, you might have heard of a library called [FluentValidation](https://fluentvalidation.net/). It has a really nice API for building up validation rules, and that made me wonder whether I could achieve something similar in TypeScript. While **fluentvalidation-ts** is not a direct port, it will still feel very familiar to anyone who's used FluentValidation before.

## Installation

You can install **fluentvalidation-ts** with NPM/Yarn, or include it directly via a `<script>` tag.

### NPM/Yarn

With NPM:

```bash
npm install fluentvalidation-ts --save
```

or Yarn:

```bash
yarn add fluentvalidation-ts
```

### CDN

To target the latest version, add the following:

```html
<script src="https://unpkg.com/fluentvalidation-ts/dist/index.global.js"></script>
```

Or, to target a specific version (e.g. `4.0.0`), add the following:

```html
<script src="https://unpkg.com/fluentvalidation-ts@4.0.0/dist/index.global.js"></script>
```

Once you've done this, all you need is the `Validator` class which can be accessed via:

```js
window['fluentvalidation'].Validator;
```

## The Gist

To use **fluentvalidation-ts** simply import the `Validator` generic class, and define your own class which extends it using the appropriate generic type argument. Build up the rules for your various properties in the constructor of your derived class, then create an instance of your class to get hold of a validator. Finally, pass an instance of your model into the `.validate` function of your validator to obtain a validation errors object.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  name: string;
  age: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('name').notEmpty().withMessage('Please enter your name');

    this.ruleFor('age')
      .greaterThanOrEqualTo(0)
      .withMessage('Please enter a non-negative number');
  }
}

const formValidator = new FormValidator();

const valid: FormModel = {
  name: 'Alex',
  age: 26,
};
formValidator.validate(valid);
// {}

const invalid: FormModel = {
  name: '',
  age: -1,
};
formValidator.validate(invalid);
// { name: 'Please enter your name', age: 'Please enter a non-negative number' }
```


## precisionScale.md

---
id: precisionScale
title: '.precisionScale'
---

The `.precisionScale` rule is used to ensure that the value of a given `number` property is permissible for the specified **precision** and **scale**.

These terms are defined as follows:

- **Precision** is the number of digits in a number.
- **Scale** is the number of digits to the right of the decimal point in a number.

:::warning

Prior to `v5.0.0` the `.precisionScale` rule was called `.scalePrecision` and the parameter naming was incorrect!

:::

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  price: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('price').precisionScale(4, 2);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ price: 10.01 });
// ✔ {}

formValidator.validate({ price: 0.001 }); // Too many digits after the decimal point
// ❌ { price: 'Value must be no more than 4 digits in total, with allowance for 2 decimals' }

formValidator.validate({ price: 100.1 }); // Too many digits (when accounting for reserved digits after the decimal point)
// ❌ { price: 'Value must be no more than 4 digits in total, with allowance for 2 decimals' }
```

## Reference

### `.precisionScale(precision: number, scale: number)`

A number validation rule which takes in an allowed precision and scale, and ensures that the value of the given property is permissible.

:::danger

Due to rounding issues with floating point numbers in JavaScript, this rule may not function as expected for large precisions/scales.

:::

### `precision`

This is the total number of digits that the value may have (taking into account the number of digits "reserved" for after the decimal point).

The maximum number of significant digits allowed before the decimal point (i.e. the integer part) can be calculated as `(precision - scale)`.

### `scale`

This is the maximum number of digits after the decimal point that the value may have.

:::note

When `precision` and `scale` are equal, the "leading zero" to the left of the decimal point is **not** counted as a digit (e.g. a value of `0.01` would be viewed as `.01`).

:::

## Example Message

> Value must not be more than `[precision]` digits in total, with allowance for `[scale]` decimals


## reactHookForm.md

---
id: reactHookForm
title: React Hook Form
---

While **fluentvalidation-ts** was originally developed with Formik integration in mind, [React Hook Form](https://react-hook-form.com/) has become increasingly popular in the React community. Thankfully, wonderful members of the community have contributed a [fluentvalidation-ts resolver](https://github.com/react-hook-form/resolvers?tab=readme-ov-file#fluentvalidation-ts) for React Hook Form, allowing you to integrate it seamlessly with no effort required on your part!


## ruleFor.md

---
id: ruleFor
title: '.ruleFor'
---

The `.ruleFor` method on the `Validator` class is used to build up rule chains for properties on your model.

To get started, simply call `this.ruleFor` in the constructor of your validator and pass in the name of a property on your model (note that this is strongly typed, you'll get a compilation error if you pass the name of a property that doesn't exist on the model).

The result of this call is a rule chain builder that exposes all the relevant built-in validation rules for the property you specified.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  name: string;
  isEmployed: boolean;
  jobTitle: string | null;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    // Returns a rule chain builder for the 'name' property
    this.ruleFor('name');
  }
}
```

To add a validation rule to the target property, simply call the relevant method on the rule chain builder (passing in any parameters as necessary). The result of such a call is again the rule chain builder, so you can specify multiple rules in a single call to `.ruleFor`.

```typescript
// The result of adding a rule is again the rule chain builder,
// so you can add multiple rules in a single call
this.ruleFor('name').notEmpty().maxLength(100);
```

After adding a rule to the chain you also gain access to a number of configuration methods which allow you to do things like specify what error should be used if the validation rule fails, and conditions under which the rules should/shouldn't run.

```typescript
this.ruleFor('name').notEmpty().maxLength(100);

// You can specify a custom error message for each rule in the chain,
// and provide a condition to determine when the rules should run
this.ruleFor('jobTitle')
  .notEmpty()
  .withMessage('Please enter a Job Title')
  .maxLength(100)
  .withMessage('Please enter no more than 100 characters')
  // highlight-next-line
  .when((formModel) => formModel.isEmployed);

// You can also provide a condition to determine when certain rules
// should not run
this.ruleFor('jobTitle')
  .equal('')
  .withMessage('You cannot enter a Job Title if you are not employed')
  // highlight-next-line
  .unless((formModel) => formModel.isEmployed);
```

As the above example illustrates, you can make several calls to `.ruleFor` for the same property. It doesn't matter how many rule chains you define for a particular property, and you don't have to define any at all if you don't need to.


## ruleForEach.md

---
id: ruleForEach
title: '.ruleForEach'
---

The `.ruleForEach` method on the `Validator` class is much like the [`.ruleFor`](api/core/ruleFor.md) method, except that is used to build up rule chains for **array** properties on your model.

You can use `.ruleForEach` to specify a rule chain that should apply to **each element** of a particular array property. Aside from this, the `.ruleForEach` method works almost exactly the same as the [`.ruleFor`](api/core/ruleFor.md) method, with all the chaining and configuration available in exactly the same way.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = { scores: Array<number> };

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleForEach('scores')
      .greaterThan(0)
      .withMessage('Please enter a positive score')
      .lessThanOrEqualTo(5)
      .withMessage('Please enter a score no greater than 5');
  }
}
```


## ruleForEachTransformed.md

---
id: ruleForEachTransformed
title: '.ruleForEachTransformed'
---

The `.ruleForEachTransformed` method on the `Validator` class is identical to the [`.ruleForEach`](api/core/ruleForEach.md) method, except that it allows you to transform each item of the given array property on your model via a transformation function prior to building up the rule chain for it.

The available validation rules will be based on the type of the **transformed** items, rather than the original type of the items.

To get started, simply call `this.ruleForTransformed` in the constructor of your validator and pass in the name of an array property on your model, along with a transformation function.

The result of this call is a rule chain builder, exactly the same as that returned by [`.ruleForEach`](api/core/ruleForEach.md), except that it exposes all the relevant built-in validation rules for the type of the **transformed** item values.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  scores: Array<string>;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleForEachTransformed('scores', (s) => Number(s))
      .must((numberScore) => !isNaN(numberScore))
      .greaterThan(0)
      .lessThanOrEqualTo(100);
  }
}
```

## Limitations

The same limitations that apply to the [`.ruleForTransformed`](api/core/ruleForTransformed.md) method apply also to the `.ruleForEachTransformed` method.


## ruleForTransformed.md

---
id: ruleForTransformed
title: '.ruleForTransformed'
---

The `.ruleForTransformed` method on the `Validator` class is identical to the [`.ruleFor`](api/core/ruleFor.md) method, except that it allows you to transform the given property on your model via a transformation function prior to building up the rule chain for it.

The available validation rules will be based on the type of the **transformed** value, rather than the original type of the property.

To get started, simply call `this.ruleForTransformed` in the constructor of your validator and pass in the name of a property on your model, along with a transformation function.

The result of this call is a rule chain builder, exactly the same as that returned by [`.ruleFor`](api/core/ruleFor.md), except that it exposes all the relevant built-in validation rules for the type of the **transformed** property value.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  quantity: string;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleForTransformed('quantity', (q) => Number(q))
      .must((numberQuantity) => !isNaN(numberQuantity))
      .greaterThan(0)
      .lessThanOrEqualTo(100);
  }
}
```

## Limitations

Note that in order to preserve the shape of the [errors object](api/core/validationErrors.md) returned by the `.validate` and `.validateAsync` methods, the transformation function passed to `.ruleForTransformed` cannot map flat types into complex types.

For example, a `string` property cannot be transformed into an `Array<string>`. This is because the errors object could then contain an array of errors at the path of the `string` property, while the expected type at this path is a "flat" error (i.e. `string | null | undefined`).

For the same reasons, complex types cannot be mapped to other complex types that look different. For example, if an `object` property is mapped to another `object` with different properties, then the errors object could contain nested errors at the path of the property with unexpected keys (i.e. keys not present on the original type of the property).

It is possible to map complex types to flat types, or complex types to other complex types with some/all of the same properties. This is because the shape of the errors object is preserved in these cases.


## setAsyncValidator.md

---
id: setAsyncValidator
title: '.setAsyncValidator'
---

The `.setAsyncValidator` rule is one of the special async rules that become available when you extend from [`AsyncValidator`](api/core/asyncValidator.md) as opposed to just [`Validator`](api/core/validator.md).

This rule works exactly the same as the [`.setValidator`](api/rules/setValidator.md) rule, except that you must pass an instance of [`AsyncValidator`](api/core/asyncValidator.md) as opposed to an instance of [`Validator`](api/core/validator.md).

As with the [`.setValidator`](api/rules/setValidator.md) rule, the async validator to use is specified by way of a producer function, which takes in the value of the base model and returns an appropriate validator.

## Examples

The documentation page for the [`.setValidator`](api/rules/setValidator.md) rule includes a full list of examples demonstrating the different overloads that are available.

These are all relevant to the `.setAsyncValidator` rule too, just replace `Validator` with `AsyncValidator` and `.setValidator` with `.setAsyncValidator`.

### Nested validator does not depend on the base model

In this example the nested validator has no dependency on the base model, so we can simply define an instance of the nested validator ahead of time and return that from the validator producer function.

```typescript
import { AsyncValidator } from 'fluentvalidation-ts';

type ContactDetails = {
  name: string;
  emailAddress: string;
};

// highlight-start
class ContactDetailsValidator extends AsyncValidator<ContactDetails> {
  constructor() {
    super();

    this.ruleFor('name').notEmpty();

    this.ruleFor('emailAddress')
      .emailAddress()
      .mustAsync(
        async (emailAddress) => await api.emailAddressNotInUse(emailAddress)
      )
      .withMessage('This email address is already in use');
  }
}

const contactDetailsValidator = new ContactDetailsValidator();
// highlight-end

type FormModel = {
  contactDetails: ContactDetails;
};

class FormValidator extends AsyncValidator<FormModel> {
  constructor() {
    super();

    // highlight-start
    this.ruleFor('contactDetails').setAsyncValidator(
      () => contactDetailsValidator
    );
    // highlight-end
  }
}

const formValidator = new FormValidator();

await formValidator.validateAsync({
  contactDetails: { name: 'Alex', emailAddress: 'alex123@example.com' },
});
// ✔ {}

await formValidator.validateAsync({
  contactDetails: { name: 'Alex', emailAddress: 'alex@example.com' },
});
// ❌ { contactDetails: { emailAddress: 'This email address is already in use' } }
```

## Reference

### `.setAsyncValidator(asyncValidatorProducer: (model: TModel) => AsyncValidator<TValue>)`

A validation rule which takes in a validator producer function and ensures that the given property is valid according to the async validator produced by that function.

### `TModel`

Matches the type of the base model.

### `TValue`

Matches the type of the property that the rule is applied to.

### `AsyncValidator`

The [`AsyncValidator`](api/core/asyncValidator.md) generic class provided by **fluentvalidation-ts**.


## setValidator.md

---
id: setValidator
title: '.setValidator'
---

The `.setValidator` rule is used to ensure that the value of a given `object` property is valid according to a given [`Validator`](api/core/validator.md).

The validator to use is specified by way of a producer function, which takes in the value of the base model and returns an appropriate validator.

This approach enables the nested validator to depend on the base model, and makes recursive validation possible.

## Examples

### Nested validator does not depend on the base model

In this example the nested validator has no dependency on the base model, so we can simply define an instance of the nested validator ahead of time and return that from the validator producer function.

```typescript
import { Validator } from 'fluentvalidation-ts';

// highlight-start
type ContactDetails = {
  name: string;
  emailAddress: string;
};

class ContactDetailsValidator extends Validator<ContactDetails> {
  constructor() {
    super();

    this.ruleFor('name').notEmpty();

    this.ruleFor('emailAddress').emailAddress();
  }
}

const contactDetailsValidator = new ContactDetailsValidator();
// highlight-end

type FormModel = {
  // highlight-next-line
  contactDetails: ContactDetails;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    // highlight-next-line
    this.ruleFor('contactDetails').setValidator(() => contactDetailsValidator);
  }
}

const formValidator = new FormValidator();

formValidator.validate({
  contactDetails: { name: 'Alex', emailAddress: 'alex@example.com' },
});
// ✔ {}

formValidator.validate({
  contactDetails: { name: '', emailAddress: 'alex@example.com' },
});
// ❌ { contactDetails: { name: 'Value cannot be empty' } }
```

### Nested validator depends on the base model

In this example the nested validator has a constructor argument which changes its behaviour.

In particular, we only require an email address to be given if the user has indicated that they wish to sign up to the mailing list.

```typescript
import { Validator } from 'fluentvalidation-ts';

type ContactDetails = {
  name: string;
  emailAddress: string | null;
};

class ContactDetailsValidator extends Validator<ContactDetails> {
  // highlight-next-line
  constructor(emailAddressIsRequired: boolean) {
    super();

    this.ruleFor('name').notEmpty();

    this.ruleFor('emailAddress')
      .notNull()
      // highlight-next-line
      .when(() => emailAddressIsRequired);

    this.ruleFor('emailAddress').emailAddress();
  }
}

type FormModel = {
  signUpToMailingList: boolean;
  contactDetails: ContactDetails;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('contactDetails').setValidator(
      // highlight-next-line
      (formModel) => new ContactDetailsValidator(formModel.signUpToMailingList)
    );
  }
}

const formValidator = new FormValidator();

formValidator.validate({
  signUpToMailingList: false,
  contactDetails: { name: 'Alex', emailAddress: null },
});
// ✔ {}

formValidator.validate({
  signUpToMailingList: true,
  contactDetails: { name: 'Alex', emailAddress: null },
});
// ❌ { contactDetails: { emailAddress: 'Value cannot be null' } }
```

### Recursive validators

In this example we deal with validating a recursive (self-referencing) model.

In particular, an employee might have a line manager, who is also an employee. This line manager might themselves have a line manager, and so on.

```typescript
import { Validator } from 'fluentvalidation-ts';

type Employee = {
  name: string;
  lineManager: Employee | null;
};

class EmployeeValidator extends Validator<Employee> {
  constructor() {
    super();

    this.ruleFor('name').notEmpty();

    // highlight-next-line
    this.ruleFor('lineManager').setValidator(() => new EmployeeValidator());
  }
}

const validator = new EmployeeValidator();

validator.validate({
  name: 'Bob',
  lineManager: {
    name: 'Alice',
    lineManager: null,
  },
});
// ✔ {}

validator.validate({
  name: 'Alex',
  lineManager: {
    name: '',
    lineManager: null,
  },
});
// ❌ { lineManager: { name: 'Value cannot be empty' } }
```

## Reference

### `.setValidator(validatorProducer: (model: TModel) => Validator<TValue>)`

A validation rule which takes in a validator producer function and ensures that the given property is valid according to the validator produced by that function.

### `TModel`

Matches the type of the base model.

### `TValue`

Matches the type of the property that the rule is applied to.

### `Validator`

The [`Validator`](api/core/validator.md) generic class provided by **fluentvalidation-ts**.


## tutorial.md

---
id: tutorial
title: Tutorial
---

## Introduction

This tutorial will walk you through some of the core features of **fluentvalidation-ts**. We'll start off with a simple form model and a correspondingly simple validator. As the tutorial goes on we'll add more fields to our form model and dive deeper into what we can do with our validator.

I recommend that you work through this tutorial in order, and follow along by running the code locally (or in an online sandbox).

## Setup

For this tutorial we'll asssume that you're using [TypeScript](https://www.typescriptlang.org/). It is still possible to use **fluentvalidation-ts** without TypeScript, but you'll lose a lot of the main benefits.

## The Basics

To begin with, let's define our form model:

```typescript
type FormModel = {
  name: string;
  age: number;
};
```

As you can see, we're imagining a very basic form with just two simple fields.

Now, let's define a validator for this form model. First we need to import the [`Validator`](api/core/validator.md) class. Add the following to the top of your file:

```typescript
import { Validator } from 'fluentvalidation-ts';
```

Once we have the `Validator` base class, we can define our own validator by extending it. Underneath where we've defined `FormModel`, add the following:

```typescript
class FormValidator extends Validator<FormModel> {}
```

Next, we can add rules for our properties in the constructor of our validator using the [`.ruleFor`](api/core/ruleFor.md) method:

```typescript
class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    // highlight-start
    this.ruleFor('name').notEmpty().maxLength(100);

    this.ruleFor('age').greaterThanOrEqualTo(0);
    // highlight-end
  }
}
```

If you're feeling adventurous, try adding another rule or two to each property.

Now, to actually validate an instance of our form model we first need to create an instance of our validator. Underneath the definition for `FormValidator` add the following:

```typescript
const formValidator = new FormValidator();
```

We can validate instances of our form model by passing them into the `.validate` method of our validator:

```typescript
const valid: FormModel = { name: 'Alex', age: 26 };
// highlight-next-line
console.log(formValidator.validate(valid)); // {}

const invalid: FormModel = { name: '', age: 26 };
// highlight-next-line
console.log(formValidator.validate(invalid)); // { name: 'Value cannot be empty' }
```

As you can see, the validation result is an object with an appropriate property for each invalid field on the given form model.

Try experimenting with some different values for each field to get used to the shape of the errors object.

## Custom Messages

All the built-in validation rules come equipped with a sensible error message that is exposed via the errors object if validation fails.

To specify your own message to be used instead, simply call [`.withMessage`](api/configuration/withMessage.md) after the rule definition and pass in your own message.

Modify the rule chain for the name property as follows:

```typescript
this.ruleFor('name')
  .notEmpty()
  // highlight-next-line
  .withMessage('Please enter your name')
  .maxLength(100);
```

Now, if we validate an invalid form model:

```typescript
const invalid: FormModel = { name: '', age: 26 };
// highlight-next-line
console.log(formValidator.validate(invalid)); // { name: 'Please enter your name' }
```

Note that `.withMessage` only applies to the rule immediately preceding it in the rule chain, not to all rules in the chain so far.

## Conditional Rules

Sometimes you only want particular rules or rule chains to apply under certain circumstances.

Let's add a couple of properties to our form model:

```typescript
type FormModel = {
  name: string;
  age: number;
  // highlight-start
  hasPet: boolean;
  nameOfPet: string | null;
  // highlight-end
};
```

If the person indicates that they have a pet then we'd like to enforce that they enter their pet's name. Likewise, if they don't have a pet we want to ensure that the pet name field is not filled out.

We can make use of the [`.when`](api/configuration/when.md) and [`.unless`](api/configuration/unless.md) configuration methods to achieve this.

Add the following to the end of your constructor (underneath the rules for the name and age fields):

```typescript
this.ruleFor('nameOfPet')
  .notNull()
  .notEmpty()
  // highlight-next-line
  .when((formModel) => formModel.hasPet);

this.ruleFor('nameOfPet')
  .null()
  // highlight-next-line
  .unless((formModel) => formModel.hasPet);
```

Now if we validate some invalid form models:

```typescript
const invalidWithPet: FormModel = {
  name: 'Alex',
  age: 26,
  // highlight-start
  hasPet: true,
  nameOfPet: '',
  // highlight-end
};
// highlight-next-line
console.log(formValidator.validate(invalidWithPet)); // { nameOfPet: 'Value cannot be empty' }

const invalidWithoutPet: FormModel = {
  name: 'Alex',
  age: 26,
  // highlight-start
  hasPet: false,
  nameOfPet: 'Doggy',
  // highlight-end
};
// highlight-next-line
console.log(formValidator.validate(invalidWithoutPet)); // { nameOfPet: 'Value must be null' }
```

In this example each condition applies to the entire rule chain preceding it, but you can also specify that a condition applies only to the preceding rule in the chain. See the relevant documentation pages for [`.when`](api/configuration/when.md) and [`.unless`](api/configuration/unless.md) to find out more.

## Collections

So far our form model has only contained simple fields, but many forms involve collections.

Let's add another field to our form model:

```typescript
type FormModel = {
  name: string;
  age: number;
  hasPet: boolean;
  nameOfPet: string | null;
  // highlight-next-line
  hobbies: Array<string>;
};
```

Now suppose we want to validate that each entry in the hobbies array is non-empty and no longer than 100 characters in length.

To achieve this we can make use of the convenient [`.ruleForEach`](api/core/ruleForEach) method.

Add the following to the end of your constructor:

```typescript
this.ruleForEach('hobbies').notEmpty().maxLength(100);
```

Now, if we validate some form models:

```typescript
const valid: FormModel = {
  name: 'Alex',
  age: 26,
  hasPet: false,
  nameOfPet: null,
  // highlight-next-line
  hobbies: ['Coding', 'Music', 'Eating'],
};
// highlight-next-line
console.log(formValidator.validate(valid)); // {}

const invalid: FormModel = {
  name: 'Alex',
  age: 26,
  hasPet: false,
  nameOfPet: null,
  // highlight-next-line
  hobbies: ['Coding', '', 'Eating'],
};
// highlight-next-line
console.log(formValidator.validate(invalid)); // { hobbies: [null, 'Value cannot be empty', null] }
```

As you can see, when a particular element of an array property is invalid, the resulting property on the errors object is itself an array, with an appropriate error message at the index of the invalid item. The value at the index of each valid item is `null`. It's worth noting that this behaviour has been specially designed to work with [Formik](https://jaredpalmer.com/formik/).

## Nested Fields

Our form model has become a bit more complex, but it still has a fairly flat structure.

Let's make the pet field an object rather than a string:

```typescript
type FormModel = {
  name: string;
  age: number;
  hasPet: boolean;
  // highlight-next-line
  pet: Pet | null;
  hobbies: Array<string>;
};

// highlight-start
type Pet = {
  name: string;
  species: string;
};
// highlight-end
```

You'll notice that we now get a compilation error in the constructor of our existing validator, because the `nameOfPet` field no longer exists. Don't worry, we'll fix that in a moment.

We can now define another validator, this time for the `Pet` type.

Add the following just above where you've defined the `FormValidator` class:

```typescript
class PetValidator extends Validator<Pet> {
  constructor() {
    super();

    this.ruleFor('name').notEmpty().maxLength(100);

    this.ruleFor('species').notEmpty().maxLength(100);
  }
}

const petValidator = new PetValidator();
```

We can specify that the pet field on our form model should be validated according to this new validator by using the [`.setValidator`](api/rules/setValidator) rule.

Modify the constructor of the `FormValidator` class by changing the rules that are targeting the non-existent `nameOfPet` field:

```typescript
this.ruleFor('pet')
  .notNull()
  .setValidator(() => petValidator)
  .when((formModel) => formModel.hasPet);

this.ruleFor('pet')
  .null()
  .unless((formModel) => formModel.hasPet);
```

Now, if we validate some form models:

```typescript
const valid: FormModel = {
  name: 'Alex',
  age: 26,
  hasPet: true,
  // highlight-next-line
  pet: { name: 'Doggy', species: 'Dog' },
  hobbies: ['Coding', 'Music', 'Eating'],
};
// highlight-next-line
console.log(formValidator.validate(valid)); // {}

const invalid: FormModel = {
  name: 'Alex',
  age: 26,
  hasPet: true,
  // highlight-next-line
  pet: { name: '', species: 'Cat' },
  hobbies: ['Coding', 'Music', 'Eating'],
};
// highlight-next-line
console.log(formValidator.validate(invalid)); // { pet: { name: 'Value cannot be empty' } }
```

As you can see, when a particular element of an object property is invalid, the resulting property on the errors object is itself an object. This object is exactly the errors object produced by validating the property according to the validator you specified.

It's worth noting that the `.setValidator` rule takes a function, a validator producer, rather than just a validator. This allows the validator to depend on the model you're validating, and makes recursive validation possible. See the documentation page for the [`.setValidator`](api/rules/setValidator) rule to find out more.

## Custom Rules

So far we've relied on the built-in validation rules to define our validation logic, but sometimes it's necessary to define your own custom validation logic.

Let's suppose that the age field is implemented on our form as a text input, so the associated property on the form model needs to be a string:

```typescript
type FormModel = {
  name: string;
  // highlight-next-line
  age: string;
  hasPet: boolean;
  pet: Pet | null;
  hobbies: Array<string>;
};
```

This change will break our validator, because the `.greaterThanOrEqualTo` rule is not appropriate for string properties.

In place of the `.greaterThanOrEqualTo` rule, we can use the [`.must`](api/rules/must) rule to define our own validation logic.

:::info

We can actually solve this problem more easily using `.ruleForTransformed`, but we'll get to that later!

:::

Modify the rule chain for the age property as follows:

```typescript
this.ruleFor('age')
  .notEmpty()
  .must((age) => !isNaN(Number(age)))
  .must((age) => Number(age) >= 0);
```

These rules validate that the age field is not empty, is numeric, and has a numeric value that is non-negative.

Now, if we validate some form models:

```typescript
const valid: FormModel = {
  name: 'Alex',
  // highlight-next-line
  age: '26',
  hasPet: true,
  pet: { name: 'Doggy', species: 'Dog' },
  hobbies: ['Coding', 'Music', 'Eating'],
};
// highlight-next-line
console.log(formValidator.validate(valid)); // {}

const invalid: FormModel = {
  name: 'Alex',
  // highlight-next-line
  age: 'foo',
  hasPet: true,
  pet: { name: 'Doggy', species: 'Dog' },
  hobbies: ['Coding', 'Music', 'Eating'],
};
// highlight-next-line
console.log(formValidator.validate(invalid)); // { age: 'Value is not valid' }
```

As you can see, the default error message for the `.must` rule isn't very descriptive, so let's add some custom error messages:

```typescript
this.ruleFor('age')
  .notEmpty()
  .must((age) => !isNaN(Number(age)))
  // highlight-next-line
  .withMessage('Please enter a number')
  .must((age) => Number(age) >= 0)
  // highlight-next-line
  .withMessage('Please enter a non-negative number');
```

Now, let's suppose we have many other forms in our application, and some of those also have numeric fields which are entered via text inputs.

Rather than repeating the validation logic across validators for our different forms, let's extract it so we can reuse it.

Add the following above the definition of the `FormValidator` class:

```typescript
const beNumeric = (value: string) => !isNaN(Number(value));

const beNonNegative = (value: string) => Number(value) >= 0;
```

Now, modify the rule chain for the age property as follows:

```typescript
this.ruleFor('age')
  .notEmpty()
  // highlight-next-line
  .must(beNumeric)
  .withMessage('Please enter a number')
  // highlight-next-line
  .must(beNonNegative)
  .withMessage('Please enter a non-negative number');
```

This hasn't really changed anything, but we can now use the logic defined in `beNumeric` and `beNonNegative` across many different validators.

This is great, but you might have noticed that we still need to define our custom messages each time. Fortunately, the `.must` rule has an override that allows us to pass in both custom validation logic and a custom message.

Change the definitions of `beNumeric` and `beNonNegative` to the following:

```typescript
const beNumeric = {
  predicate: (value: string) => !isNaN(Number(value)),
  message: 'Please enter a number',
};

const beNonNegative = {
  predicate: (value: string) => Number(value) >= 0,
  message: 'Please enter a non-negative number',
};
```

We can now remove the calls to `.withMessage` in our rule chain:

```typescript
this.ruleFor('age')
  .notEmpty()
  // highlight-start
  .must(beNumeric)
  .must(beNonNegative);
// highlight-end
```

As you can see, the `.must` rule is a very powerful tool, and this example only scratches the surface of what's possible with it. For full details, see the [documentation page](api/rules/must).

## Transformed Values

As we saw in the previous example, sometimes we need to transform the value of a property before validating it. This is especially common when dealing with text inputs, where the value is always a string.

Since this is such a common scenario, **fluentvalidation-ts** provides a convenient [`.ruleForTransformed`](api/core/ruleForTransformed) method that allows us to define a transformation function for the value of a property before applying validation rules.

Let's modify our `FormValidator` class to use this method for the age property:

```typescript
const numberOrNull = (value: string) =>
  isNaN(Number(value)) ? null : Number(value);

this.ruleForTransformed('age', numberOrNull)
  .notNull()
  .withMessage('Please enter a number')
  .greaterThanOrEqualTo(0)
  .withMessage('Please enter a non-negative number');
```

Now, if we validate some form models:

```typescript
const valid: FormModel = {
  name: 'Alex',
  // highlight-next-line
  age: '26',
  hasPet: true,
  pet: { name: 'Doggy', species: 'Dog' },
  hobbies: ['Coding', 'Music', 'Eating'],
};
// highlight-next-line
console.log(formValidator.validate(valid)); // {}

const invalid1: FormModel = {
  name: 'Alex',
  // highlight-next-line
  age: '-10',
  hasPet: true,
  pet: { name: 'Doggy', species: 'Dog' },
  hobbies: ['Coding', 'Music', 'Eating'],
};
// highlight-next-line
console.log(formValidator.validate(invalid2)); // { age: 'Please enter a non-negative number' }

const invalid2: FormModel = {
  name: 'Alex',
  // highlight-next-line
  age: 'foo',
  hasPet: true,
  pet: { name: 'Doggy', species: 'Dog' },
  hobbies: ['Coding', 'Music', 'Eating'],
};
// highlight-next-line
console.log(formValidator.validate(invalid2)); // { age: 'Please enter a number' }
```

Of course, this is an overly simplified example, and the eagle-eyed among you will have noticed that there are several edge cases we're skimming over here.

In general, you will likely want to define more robust transformation functions the handle such edge cases, and reuse them across your validators.

Note that there is an analagous [`.ruleForEachTransformed`](api/core/ruleForEachTransformed) method that works exactly the same, but for collections.


## undefined.md

---
id: undefinedRule
title: '.undefined'
---

The `.undefined` rule is used to ensure that the value of a given property is `undefined`.

:::note

Note that this rule considers `null` values to be **invalid**. If you need to allow for both `null` and `undefined` values (or just `null` values), you may use the [`.null`](./null.md) rule instead.

:::

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  customerId?: number | null;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('customerId').undefined();
  }
}

const formValidator = new FormValidator();

formValidator.validate({});
// ✔ {}

formValidator.validate({ customerId: undefined });
// ✔ {}

formValidator.validate({ customerId: 100 });
// ❌ { customerId: 'Value must be undefined' }

formValidator.validate({ customerId: null });
// ❌ { customerId: 'Value must be undefined' }
```

## Reference

### `.undefined()`

A validation rule which ensures that the given property is `undefined`.

## Example Message

> Value must be undefined


## unless.md

---
id: unless
title: '.unless'
---

The `.unless` option is used to control when a rule or chain of rules should **not** execute.

By default, the `.unless` option will apply to all rules in the chain so far, but you can pass a second parameter to specify that it should only apply to the rule immediately preceding it.

:::note

In the case that there are multiple `.when` and/or `.unless` conditions in the rule chain, each condition applies only to the rules defined **between it and the previous condition**.

:::

## Examples

### Apply to all rules in the chain so far

In this example we apply an `.unless` condition to an entire rule chain.

In particular, we validate that the delivery note has been entered and is no more than 1,000 characters long unless it has been specified that a delivery note is not required.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  doesNotRequireDeliveryNote: boolean;
  deliveryNote: string | null;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('deliveryNote')
      .notNull()
      .notEmpty()
      .maxLength(1000)
      // highlight-next-line
      .unless((formModel) => formModel.doesNotRequireDeliveryNote);
  }
}

const formValidator = new FormValidator();

formValidator.validate({
  doesNotRequireDeliveryNote: true,
  deliveryNote: null,
});
// ✔ {}

formValidator.validate({
  doesNotRequireDeliveryNote: false,
  deliveryNote: null,
});
// ❌ { deliveryNote: 'Value cannot be null' }
```

### Multiple calls within the same chain

In this example we apply multiple `.unless` conditions within the same rule chain.

In particular, we validate that the account balance is non-negative unless overdrafts are allowed, and also validate that the account balance is more than 100 unless the account is not subject to minimum balance requirements.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  accountBalance: number;
  allowOverdrafts: boolean;
  subjectToMinimumBalance: boolean;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('accountBalance')
      .greaterThanOrEqualTo(0)
      // highlight-next-line
      .unless((formModel) => formModel.allowOverdrafts)
      .greaterThanOrEqualTo(100)
      // highlight-next-line
      .unless((formModel) => !formModel.subjectToMinimumBalance);
  }
}

const formValidator = new FormValidator();

formValidator.validate({
  accountBalance: -50,
  allowOverdrafts: true,
  subjectToMinimumBalance: false,
});
// ✔ {}

formValidator.validate({
  accountBalance: -50,
  allowOverdrafts: false,
  subjectToMinimumBalance: false,
});
// ❌ { accountBalance: 'Value must be greater than or equal to 0' }

formValidator.validate({
  accountBalance: 50,
  allowOverdrafts: false,
  subjectToMinimumBalance: true,
});
// ❌ { accountBalance: 'Value must be greater than or equal to 100' }
```

### Apply to a specific rule in the chain

In this example we apply an `.unless` condition to a specific rule in the chain.

In particular, we validate that an age has been entered, and also validate that it is at least 18 unless no alcoholic drink has been chosen.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  age: number | null;
  alcoholicDrink: string | null;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('age')
      .notNull()
      .greaterThanOrEqualTo(18)
      // highlight-start
      .unless((formModel) => formModel.alcoholicDrink == null, 'AppliesToCurrentValidator');
    // highlight-end
  }
}

const formValidator = new FormValidator();

formValidator.validate({
  age: 17,
  alcoholicDrink: null,
});
// ✔ {}

formValidator.validate({
  age: 17,
  alcoholicDrink: 'Beer',
});
// ❌ { age: 'Value must be greater than or equal to 18' }

formValidator.validate({
  age: null,
  alcoholicDrink: null,
});
// ❌ { age: 'Value cannot be null' }
```

## Reference

### `.unless(condition: (model: TModel) => boolean, appliesTo?: 'AppliesToAllValidators' | 'AppliesToCurrentValidator')`

A configuration option which controls when a particular rule or chain of rules should not execute.

### `condition`

This is a function which accepts the value of the base model and returns a `boolean` indicating whether the rule or chain of rules preceding it should not execute.

A return value of `true` indicates that the rule or chain of rules **should not** execute.

Conversely, a return value of `false` indicates that the rule or chain of rules **should** execute.

### `TModel`

Matches the type of the base model.

### `appliesTo`

This is an optional parameter which can be used to control which rules in the current rule chain the condition applies to.

A value of `'AppliesToAllValidators'` means that the `.unless` condition applies to all rules in the current rule chain so far. If there are other calls to `.when` or `.unless` in the chain, only the rules defined since the most recent condition will have the condition applied to them.

A value of `'AppliesToCurrentValidator'` specifies that the `.unless` condition only controls the execution of the rule immediately preceding it in the current rule chain.

By default, the `appliesTo` parameter is set to `'AppliesToAllValidators'`.


## validationErrors.md

---
id: validationErrors
title: ValidationErrors
---

Calling `.validate` on an instance of a validator returns an object of type `ValidationErrors<TModel>`, which represents the validity of the model that was passed in, and exposes relevant error messages.

Consider the following example:

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  name: string;
  age: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('name').notEmpty().withMessage('Please enter your name');

    this.ruleFor('age')
      .greaterThanOrEqualTo(0)
      .withMessage('Your age must be a positive number');
  }
}

const formValidator = new FormValidator();
```

Each property on the model is either valid or invalid according to the validator. If a property is valid then there will be no corresponding property on the validation errors object.

```typescript
formValidator.validate({ name: 'Alex', age: 26 });
// {}
```

On the other hand, if a property is invalid then there will be a corresponding property on the validation errors object.

For simple properties (`string`, `boolean`, `number`, etc.) the value of this corresponding property is always a `string` error message.

```typescript
formValidator.validate({ name: '', age: 26 });
// { name: 'Please enter your name' }
```

In any case, the error results from the **first** failing validation rule for that property.

> Being aware of this behaviour is very important, as it can influence the order in which you wish to define rules for your properties.

For object properties and array properties the corresponding value on the errors object looks slightly different - in particular it is not always a `string`.

## Object properties

Object properties contain nested properties, which could have validation rules of their own (by way of the `.setValidator` rule). As a result, validation for the base model could fail because a nested property on a particular object property is invalid.

Consider the following example:

```typescript
import { Validator } from 'fluentvalidation-ts';

type ContactDetails = {
  name: string;
  emailAddress: string;
};

class ContactDetailsValidator extends Validator<ContactDetails> {
  constructor() {
    super();

    this.ruleFor('name').notEmpty().withMessage('Please enter your name');

    this.ruleFor('emailAddress')
      .emailAddress()
      .withMessage('Please enter a valid email address');
  }
}

const contactDetailsValidator = new ContactDetailsValidator();

type FormModel = { contactDetails: ContactDetails };

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('contactDetails').setValidator(() => contactDetailsValidator);
  }
}

const formValidator = new FormValidator();
```

In this example the base model has a `contactDetails` property on it, which is an object (of type `ContactDetails`). This property is validated according to the `ContactDetailsValidator` validator.

If a valid value is provided, then the errors object has no corresponding property on it (as with simple properties).

```typescript
formValidator.validate({
  contactDetails: { name: 'Alex', emailAddress: 'alex@example.com' },
});
// {}
```

However, if an invalid value is provided, then the errors object has a corresponding property on it. The value of this property is essentially the errors object that you get by calling `.validate` on `contactDetailsValidator` with the value of `contactDetails`.

```typescript
formValidator.validate({
  contactDetails: { name: '', emailAddress: 'alex@example.com' },
});
// { contactDetails: { name: 'Please enter your name' } }
```

With **fluentvalidation-ts** you can have arbitrary levels of nested object properties on your model and the resulting errors object will have a corresponding structure.

It's worth pointing out that if you specify a rule on an object property directly (i.e. a rule other than the `.setValidator` rule) then you'll end up with a `string` value in the errors object if that validation rule fails (and is before any failing `.setValidator` rules in the chain).

```typescript
import { Validator } from 'fluentvalidation-ts';

type ContactDetails = {
  name: string;
  emailAddress: string;
};

class ContactDetailsValidator extends Validator<ContactDetails> {
  constructor() {
    super();

    this.ruleFor('name').notEmpty().withMessage('Please enter your name');

    this.ruleFor('emailAddress')
      .emailAddress()
      .withMessage('Please enter a valid email address');
  }
}

const contactDetailsValidator = new ContactDetailsValidator();

type FormModel = { contactDetails: ContactDetails | null };

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('contactDetails')
      .notNull() // <--- If this rule fails we'll get a `string` error in the errors object
      .setValidator(() => contactDetailsValidator);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ contactDetails: null });
// { contactDetails: 'Value cannot be null' }
```

## Array properties

Array properties can contain an arbitrary number of elements, each of which could be valid or invalid at an item level (by way of a rule defined in a `.ruleForEach` rule chain). As a result, validation for the array property could fail because a particular element in the array is invalid.

Consider the following example:

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = { scores: Array<number> };

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleForEach('scores').inclusiveBetween(1, 10);
  }
}

const formValidator = new FormValidator();
```

In this example the base model has a `scores` property on it, and we've specified that each element within that array should be between `1` and `10` (inclusive).

If a valid value is provided, then the errors object has no corresponding property on it (as with simple properties).

```typescript
formValidator.validate({ scores: [1, 3, 4, 9] });
// {}
```

However, if an invalid value is provided, then the errors object has a corresponding property on it. The value of this property is an array where each element is either `null` (if the corresponding element is valid) or an appropriate errors object (if the corresponding element is invalid).

In this case, each element of the array property is a flat type (`number`), so any corresponding errors will just be of type `string`.

```typescript
formValidator.validate({ scores: [1, -3, 4, 11] });
/*
  {
    scores: [
      null,
      'Value must be between 1 and 10 (inclusive)',
      null,
      'Value must be between 1 and 10 (inclusive)'
    ],
  }
*/
```

In this example we have an array of `number` elements, but we could just as easily have an array of `object` elements which might be validated by way of a `.setValidator` rule. In that case, the elements in the array on the errors object could themselves be of type `object` (as explained above).

It's worth pointing out that if you specify a rule on an array property directly (i.e. a rule defined via `.ruleFor` as opposed to `.ruleForEach`) then you'll end up with a `string` value in the errors object if that validation rule fails.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = { scores: Array<number> };

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('scores')
      .must((scores) => scores.length > 0)
      .withMessage('Cannot be empty');

    this.ruleForEach('scores').inclusiveBetween(1, 10);
  }
}

const formValidator = new FormValidator();

formValidator.validate({ scores: [] });
// { scores: 'Cannot be empty' }
```


## validator.md

---
id: validator
title: Validator
---

## Validator&lt;TModel&gt;

The `Validator<TModel>` generic class is the core component of the **fluentvalidation-ts** API.

```typescript
import { Validator } from 'fluentvalidation-ts';
```

To define a validator for a model of type `TModel` all you have to do is define a class which extends `Validator<TModel>` and specify some rules in the constructor using the `.ruleFor` and `.ruleForEach` methods.

```typescript
type FormModel = { name: string };

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('name').notEmpty().withMessage('Please enter your name');
  }
}
```

## .validate

To actually validate an instance of your model, simply create an instance of your validator and pass your model to the `.validate` method.

```typescript
const formValidator = new FormValidator();

const validResult = formValidator.validate({ name: 'Alex' });
// ✔ {}

const invalidResult = formValidator.validate({ name: '' });
// ❌ { name: 'Please enter your name' }
```

A call to `.validate` returns an object of type `ValidationErrors<TModel>`, which describes the validity of the given value.


## when.md

---
id: when
title: '.when'
---

The `.when` option is used to control when a rule or chain of rules should execute.

By default, the `.when` option will apply to all rules in the chain so far, but you can pass a second parameter to specify that it should only apply to the rule immediately preceding it.

:::note

In the case that there are multiple `.when` and/or `.unless` conditions in the rule chain, each condition applies only to the rules defined **between it and the previous condition**.

:::

## Examples

### Apply to all rules in the chain so far

In this example we apply a `.when` condition to an entire rule chain.

In particular, we validate that the delivery note has been entered and is no more than 1,000 characters long when it has been specified that a delivery note is required.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  requiresDeliveryNote: boolean;
  deliveryNote: string | null;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('deliveryNote')
      .notNull()
      .notEmpty()
      .maxLength(1000)
      // highlight-next-line
      .when((formModel) => formModel.requiresDeliveryNote);
  }
}

const formValidator = new FormValidator();

formValidator.validate({
  requiresDeliveryNote: false,
  deliveryNote: null,
});
// ✔ {}

formValidator.validate({
  requiresDeliveryNote: true,
  deliveryNote: null,
});
// ❌ { deliveryNote: 'Value cannot be null' }
```

### Multiple calls within the same chain

In this example we apply multiple `.when` conditions within the same rule chain.

In particular, we validate that Sunday delivery rates are only applied when the delivery day is a Sunday.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  deliveryDay: string;
  deliveryRate: number;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('deliveryRate')
      .equal(4.99)
      .withMessage('Sunday rates must apply if delivery day is Sunday')
      // highlight-next-line
      .when((formModel) => formModel.deliveryDay === 'Sunday')
      .equal(2.99)
      .withMessage('Standard rates must apply if delivery day is Monday to Saturday')
      // highlight-next-line
      .when((formModel) => formModel.deliveryDay !== 'Sunday');
  }
}

const formValidator = new FormValidator();

formValidator.validate({ deliveryDay: 'Sunday', deliveryRate: 4.99 });
// ✔ {}

formValidator.validate({ deliveryDay: 'Sunday', deliveryRate: 2.99 });
// ❌ { deliveryRate: 'Sunday rates must apply if delivery day is Sunday' }

formValidator.validate({ deliveryDay: 'Monday', deliveryRate: 2.99 });
// ✔ {}

formValidator.validate({ deliveryDay: 'Monday', deliveryRate: 4.99 });
// ❌ { deliveryRate: 'Standard rates must apply if delivery day is Monday to Saturday' }
```

### Apply to a specific rule in the chain

In this example we apply a `.when` condition to a specific rule in the chain.

In particular, we validate that an age has been entered, and also validate that it is at least 18 when an alcoholic drink has been chosen.

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  age: number | null;
  alcoholicDrink: string | null;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('age')
      .notNull()
      .greaterThanOrEqualTo(18)
      // highlight-start
      .when((formModel) => formModel.alcoholicDrink != null, 'AppliesToCurrentValidator');
    // highlight-end
  }
}

const formValidator = new FormValidator();

formValidator.validate({
  age: 17,
  alcoholicDrink: null,
});
// ✔ {}

formValidator.validate({
  age: 17,
  alcoholicDrink: 'Beer',
});
// ❌ { age: 'Value must be greater than or equal to 18' }

formValidator.validate({
  age: null,
  alcoholicDrink: null,
});
// ❌ { age: 'Value cannot be null' }
```

## Reference

### `.when(condition: (model: TModel) => boolean, appliesTo?: 'AppliesToAllValidators' | 'AppliesToCurrentValidator')`

A configuration option which controls when a particular rule or chain of rules should execute.

### `condition`

This is a function which accepts the value of the base model and returns a `boolean` indicating whether the rule or chain of rules preceding it should execute.

A return value of `true` indicates that the rule or chain of rules **should** execute.

Conversely, a return value of `false` indicates that the rule or chain of rules **should not** execute.

### `TModel`

Matches the type of the base model.

### `appliesTo`

This is an optional parameter which can be used to control which rules in the current rule chain the condition applies to.

A value of `'AppliesToAllValidators'` means that the `.when` condition applies to all rules in the current rule chain so far. If there are other calls to `.when` or `.unless` in the chain, only the rules defined since the most recent condition will have the condition applied to them.

A value of `'AppliesToCurrentValidator'` specifies that the `.when` condition only controls the execution of the rule immediately preceding it in the current rule chain.

By default, the `appliesTo` parameter is set to `'AppliesToAllValidators'`.


## withMessage.md

---
id: withMessage
title: '.withMessage'
---

The `.withMessage` option is used to specify a custom error message that should be used when a given validation rule fails.

All validation rules have a default error message associated with them, but sometimes you may wish to override these defaults and specify your own user-friendly error message.

Note that `.withMessage` only applies to the rule immediately preceding it in the rule chain, not to all rules in the chain so far.

## Example

```typescript
import { Validator } from 'fluentvalidation-ts';

type FormModel = {
  name: string;
};

class FormValidator extends Validator<FormModel> {
  constructor() {
    super();

    this.ruleFor('name')
      .notEmpty()
      // highlight-next-line
      .withMessage('Please enter your name')
      .maxLength(1000)
      // highlight-next-line
      .withMessage('Please enter no more than 1,000 characters');
  }
}

const formValidator = new FormValidator();

formValidator.validate({ name: 'Alex' });
// ✔ {}

formValidator.validate({ name: '' });
// ❌ { name: 'Please enter your name' }
```

## Reference

### `.withMessage(customMessage: string)`

A configuration option which takes a custom error message and uses that message in place of the default error message if the given validation rule fails.
