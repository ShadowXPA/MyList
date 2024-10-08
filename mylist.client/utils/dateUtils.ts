export function parseDate(dateStr: string): Date {
  return new Date(dateStr.endsWith('Z') ? dateStr : (dateStr + 'Z'))
}