import * as React from "react";
import { useState, useEffect } from "react";
import { Plus, X } from "lucide-react";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import { Card, CardHeader, CardTitle, CardContent } from "./ui/card";
import type { Contact, CreateContactDto, UpdateContactDto } from "../types";

interface FormErrors {
  name?: string;
  age?: string;
  phones?: string;
}

interface ContactFormProps {
  contact?: Contact | null;
  onSubmit: (data: CreateContactDto | UpdateContactDto) => void;
  onCancel: () => void;
  isLoading?: boolean;
}

export function ContactForm({
  contact,
  onSubmit,
  onCancel,
  isLoading,
}: ContactFormProps) {
  const [name, setName] = useState("");
  const [age, setAge] = useState("");
  const [phoneNumbers, setPhoneNumbers] = useState<string[]>([""]);
  const [errors, setErrors] = useState<FormErrors>({});
  const [touched, setTouched] = useState<Record<string, boolean>>({});

  useEffect(() => {
    if (contact) {
      setName(contact.name);
      setAge(String(contact.age));
      setPhoneNumbers(contact.phones.map((p) => p.phoneNumber));
    } else {
      setName("");
      setAge("");
      setPhoneNumbers([""]);
    }
    setErrors({});
    setTouched({});
  }, [contact]);

  const validate = (): FormErrors => {
    const newErrors: FormErrors = {};

    if (!name.trim()) {
      newErrors.name = "Name is required";
    } else if (name.trim().length > 200) {
      newErrors.name = "Name must be less than 200 characters";
    }

    const ageNum = parseInt(age, 10);
    if (!age) {
      newErrors.age = "Age is required";
    } else if (isNaN(ageNum) || ageNum < 1 || ageNum > 149) {
      newErrors.age = "Age must be between 1 and 149";
    }

    const validPhones = phoneNumbers.filter((p) => p.trim() !== "");
    if (validPhones.length === 0) {
      newErrors.phones = "At least one phone number is required";
    } else if (validPhones.some((p) => p.length > 20)) {
      newErrors.phones = "Phone numbers must be less than 20 characters";
    }

    return newErrors;
  };

  const handleBlur = (field: string) => {
    setTouched((prev) => ({ ...prev, [field]: true }));
    setErrors(validate());
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const validationErrors = validate();
    setErrors(validationErrors);
    setTouched({ name: true, age: true, phones: true });

    if (Object.keys(validationErrors).length > 0) {
      return;
    }

    onSubmit({
      name: name.trim(),
      age: parseInt(age, 10),
      phoneNumbers: phoneNumbers.filter((p) => p.trim() !== ""),
    });
  };

  const addPhone = () => {
    setPhoneNumbers([...phoneNumbers, ""]);
  };

  const removePhone = (index: number) => {
    if (phoneNumbers.length > 1) {
      setPhoneNumbers(phoneNumbers.filter((_, i) => i !== index));
    }
  };

  const updatePhone = (index: number, value: string) => {
    const updated = [...phoneNumbers];
    updated[index] = value;
    setPhoneNumbers(updated);

    if (touched.phones) {
      setErrors(validate());
    }
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle>{contact ? "Edit Contact" : "Add Contact"}</CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4" noValidate>
          <div className="space-y-2">
            <label htmlFor="name" className="text-sm font-medium">
              Name
            </label>
            <Input
              id="name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              onBlur={() => handleBlur("name")}
              placeholder="Enter name"
              maxLength={200}
              aria-invalid={touched.name && !!errors.name}
              aria-describedby={errors.name ? "name-error" : undefined}
              className={
                touched.name && errors.name ? "border-destructive" : ""
              }
            />
            {touched.name && errors.name && (
              <p id="name-error" className="text-sm text-destructive">
                {errors.name}
              </p>
            )}
          </div>

          <div className="space-y-2">
            <label htmlFor="age" className="text-sm font-medium">
              Age
            </label>
            <Input
              id="age"
              type="number"
              value={age}
              onChange={(e) => setAge(e.target.value)}
              onBlur={() => handleBlur("age")}
              placeholder="Enter age"
              min={1}
              max={149}
              aria-invalid={touched.age && !!errors.age}
              aria-describedby={errors.age ? "age-error" : undefined}
              className={touched.age && errors.age ? "border-destructive" : ""}
            />
            {touched.age && errors.age && (
              <p id="age-error" className="text-sm text-destructive">
                {errors.age}
              </p>
            )}
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium">Phone Numbers</label>
            {phoneNumbers.map((phone, index) => (
              <div key={index} className="flex gap-2">
                <Input
                  value={phone}
                  onChange={(e) => updatePhone(index, e.target.value)}
                  onBlur={() => handleBlur("phones")}
                  placeholder="Enter phone number"
                  maxLength={20}
                  aria-label={`Phone number ${index + 1}`}
                  className={
                    touched.phones && errors.phones ? "border-destructive" : ""
                  }
                />
                {phoneNumbers.length > 1 && (
                  <Button
                    type="button"
                    variant="outline"
                    size="icon"
                    onClick={() => removePhone(index)}
                    aria-label={`Remove phone number ${index + 1}`}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                )}
              </div>
            ))}
            {touched.phones && errors.phones && (
              <p className="text-sm text-destructive">{errors.phones}</p>
            )}
            <Button
              type="button"
              variant="outline"
              onClick={addPhone}
              className="w-full"
            >
              <Plus className="h-4 w-4 mr-2" /> Add Phone
            </Button>
          </div>

          <div className="flex gap-2 pt-4">
            <Button type="submit" disabled={isLoading} className="flex-1">
              {isLoading ? "Saving..." : contact ? "Update" : "Add"}
            </Button>
            <Button type="button" variant="outline" onClick={onCancel}>
              Cancel
            </Button>
          </div>
        </form>
      </CardContent>
    </Card>
  );
}
