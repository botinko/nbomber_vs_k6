import http from 'k6/http';
import { check, sleep } from 'k6';

const TARGET_URL = __ENV.TARGET_URL || 'http://rust-app:8080';

export const options = {
  scenarios: {
    get_person: {
      executor: 'ramping-arrival-rate',
      startRate: 0,
      maxVUs: 500,
      stages: [
        { duration: '3m', target: 5000 },
      ],
      exec: 'getPerson',
    },
    post_person: {
      executor: 'ramping-arrival-rate',
      startRate: 0,
      maxVUs: 500,
      stages: [
        { duration: '3m', target: 5000 },
      ],
      exec: 'postPerson',
    },
  },
  // Reduce CPU usage
  noConnectionReuse: false,
  discardResponseBodies: true,
};

export function getPerson() {
  const res = http.get(`${TARGET_URL}/api/person`, {
    tags: { name: 'get_person' },
  });
  
  check(res, {
    'GET status is 200': (r) => r.status === 200,
  });
}

const payload = JSON.stringify({
  id: 1,
  name: 'Jane Smith',
  email: 'jane.smith@test.com',
  age: 28,
  city: 'San Francisco',
  occupation: 'DevOps Engineer',
});

export function postPerson() {
  const res = http.post(`${TARGET_URL}/api/person`, payload, {
    headers: {
      'Content-Type': 'application/json',
    },
    tags: { name: 'post_person' },
  });
  
  check(res, {
    'POST status is 200': (r) => r.status === 200,
  });
}

export function setup() {
  // Wait for service to be ready
  let attempts = 0;
  while (attempts < 30) {
    try {
      const res = http.get(`${TARGET_URL}/health`);
      if (res.status === 200) {
        console.log('Target service is ready');
        return;
      }
    } catch (e) {
      console.log(`Waiting for target service... (attempt ${attempts + 1})`);
    }
    sleep(2);
    attempts++;
  }
  throw new Error('Target service did not become ready in time');
}
