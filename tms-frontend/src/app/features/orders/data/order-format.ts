const PRICE_FORMAT = new Intl.NumberFormat('de-DE', { style: 'currency', currency: 'EUR' });
const DATE_FORMAT = new Intl.DateTimeFormat('de-DE', { dateStyle: 'medium', timeStyle: 'short' });

export function formatPrice(value: number): string {
  return PRICE_FORMAT.format(value);
}

export function formatDate(iso: string): string {
  return DATE_FORMAT.format(new Date(iso));
}
